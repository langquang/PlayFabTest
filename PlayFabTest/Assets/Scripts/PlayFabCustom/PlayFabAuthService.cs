using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.SharedModels;
using UnityEngine;

namespace PlayFabCustom
{
    public class PlayFabAuthService
    {
        private const string PREFS_TOKEN_KEY = "tokens"; // key in PlayerPrefs

        private const string DIC_MASTER_SERVER_ID  = "masterId";      // key in _tokens, present id of Master Account
        private const string DIC_CURRENT_SERVER_ID = "currentServer"; // key in _tokens, present current server name

        private readonly Dictionary<string, string> _tickets = new Dictionary<string, string>();

        private delegate void RequestSuccessEvent(PlayFabResultCommon result);

        private delegate void LoginSuccessEvent(LoginResult result, CreateParams createParams);

        private delegate void RequestFailureEvent(PlayFabError error);

        private delegate void CreateNodeAccountEvent(LoginResult result);

        private delegate void CreateMasterAccountEvent(LoginResult result);

        private delegate void FatalErrorEvent(LoginResult result);

        private event RequestSuccessEvent OnRequestSuccess;
        private event RequestFailureEvent OnRequestFailure;
        private event LoginSuccessEvent OnLoginSuccess;
        private event CreateNodeAccountEvent OnCreateNodeAccount;
        private event CreateMasterAccountEvent OnCreateMasterAccount;
        private event FatalErrorEvent OnFatalError;

        private string _masterPlayFabId;

        #region SINGLETON

        public static PlayFabAuthService Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new PlayFabAuthService();
                }

                return _instance;
            }
        }

        private static PlayFabAuthService _instance;

        public PlayFabAuthService()
        {
            _instance = this;
        }

        #endregion

        #region CONTROL FUNTIONS 

        public void Start()
        {
            OnRequestSuccess += OnRequestSuccessHandler;
            OnRequestFailure += OnRequestFailureHandler;
            LoadFromLocal();
            LoginFromCacheOrAutoSignUp();
        }

        public void Stop()
        {
            OnRequestSuccess -= OnRequestSuccessHandler;
            OnRequestFailure -= OnRequestFailureHandler;
            SaveToLocal();
        }

        #endregion

        #region LOCAL STORAGE OPERATIONS

        private void SaveToLocal()
        {
            var rawData = JsonConvert.SerializeObject(_tickets);
            PlayerPrefs.SetString(PREFS_TOKEN_KEY, rawData);
        }

        private void LoadFromLocal()
        {
            var rawData = PlayerPrefs.GetString(PREFS_TOKEN_KEY); // Read data from local store
            if (!string.IsNullOrEmpty(rawData))
            {
                // try to parse raw data to dictionary
                Dictionary<string, string> deserializeObj = null;
                try
                {
                    deserializeObj = JsonConvert.DeserializeObject<Dictionary<string, string>>(rawData);
                }
                catch (Exception)
                {
                    Debug.LogError("Can not Deserialize Tokens");
                }

                if (deserializeObj != null)
                {
                    // deserialize success
                    foreach (var kvp in deserializeObj)
                    {
                        _tickets[kvp.Key] = kvp.Value;
                    }
                }
            }
        }

        #endregion

        #region INTERNAL EVENTS

        private void OnRequestSuccessHandler(PlayFabResultCommon result)
        {
        }

        private void OnRequestFailureHandler(PlayFabError error)
        {
            Debug.LogError(error.ToString());
        }

        private void OnLoginSuccessHandler(LoginResult loginResult, CreateParams createParams)
        {
            Debug.Log("PlayFab login successful!");
            var isSuccessful = true;
            if (loginResult.NewlyCreated) // Create new account
            {
                if (createParams.isCreateMaster)
                {
                    // service master account
                    _tickets[DIC_MASTER_SERVER_ID] = createParams.server;
                    OnCreateMasterAccount?.Invoke(loginResult);
                }
                else
                {
                    OnCreateNodeAccount?.Invoke(loginResult);
                }
            }
            else // Login to exist account
            {
                var curServerID = PFHelper.FindServerFromStatistic(loginResult.InfoResultPayload.PlayerStatistics);
                if (!string.IsNullOrEmpty(curServerID))
                {
                    _tickets[DIC_CURRENT_SERVER_ID] = curServerID;     // set current server
                    _tickets[curServerID] = loginResult.SessionTicket; // Store current ticket
                }
                else
                {
                    Debug.LogError("Can not parse current ServerID");
                    OnFatalError?.Invoke(loginResult);
                    isSuccessful = false;
                }
            }

            if (isSuccessful)
            {
                SaveToLocal();
                // Todo: Butin change current Session Ticket of PlayFab Sdk
            }

        }

        #endregion

        /// <summary>
        /// Call in login
        /// </summary>
        private void LoginFromCacheOrAutoSignUp()
        {
            string customId;
            if (_tickets.ContainsKey(DIC_CURRENT_SERVER_ID)) // login from cache
            {
                var server = _tickets[DIC_CURRENT_SERVER_ID];
                customId = _tickets[server];
                LoginWithCustomID(customId);
            }
            else // sign-up new account
            {
                customId = Guid.NewGuid().ToString();
                var suggestServer = "0"; // todo: set suggestServer
                LoginWithCustomID(customId, new CreateParams {isCreateMaster = true, server = suggestServer});
            }
        }

        public void LoginWithCustomID(string customId, CreateParams createParams = null, Action<LoginResult> resultCallback = null, Action<PlayFabError> errorCallback = null)
        {
            // Login with custom id on UNITY_EDITOR
            PlayFabClientAPI.LoginWithCustomID(new LoginWithCustomIDRequest
                {
                    CreateAccount = true,
                    CustomId = customId,
                    TitleId = PlayFabSettings.TitleId,
                    InfoRequestParameters = PFHelper.loginInfoRequestParams
                },
                res =>
                {
                    resultCallback?.Invoke(res);
                    OnRequestSuccess?.Invoke(res);
                    OnLoginSuccessHandler(res, createParams);
                },
                err =>
                {
                    errorCallback?.Invoke(err);
                    OnRequestFailure?.Invoke(err);
                });
        }

        public void SwitchServer(string serverId)
        {
            if (_tickets.ContainsKey(serverId)) // already create account
            {
                LoginWithCustomID(_tickets[serverId], null, result => { }, error => { });
            }
            else // create new node account
            {
                LoginWithCustomID($"{serverId}-{Guid.NewGuid().ToString()}", null, result => { }, error => { });
            }
        }
    }
}
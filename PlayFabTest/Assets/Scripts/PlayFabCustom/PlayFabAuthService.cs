using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.SharedModels;
using PlayFabCustom.Models;
using UnityEngine;

namespace PlayFabCustom
{
    public class PlayFabAuthService
    {
        private const string PREFS_TOKEN_KEY = "tokens"; // key in PlayerPrefs

        public Action<PlayFabResultCommon>       OnRequestSuccess;
        public Action<PlayFabError>              OnRequestFailure;
        public Action<LoginResult, CreateParams> OnLoginSuccess;
        public Action<LoginResult, CreateParams> OnCreateNodeAccount;
        public Action<LoginResult, CreateParams> OnCreateMasterAccount;
        public Action<LoginResult>               OnFatalError;

        private ClusterAccount _clusterAccount;

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
            _clusterAccount = new ClusterAccount();
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
            var rawData = JsonConvert.SerializeObject(_clusterAccount);
            PlayerPrefs.SetString(PREFS_TOKEN_KEY, rawData);
        }

        private void LoadFromLocal()
        {
            var rawData = PlayerPrefs.GetString(PREFS_TOKEN_KEY); // Read data from local store
            if (!string.IsNullOrEmpty(rawData))
            {
                // try to parse raw data to dictionary
                try
                {
                    _clusterAccount = JsonConvert.DeserializeObject<ClusterAccount>(rawData);
                }
                catch (Exception)
                {
                    Debug.LogError("Can not Deserialize Tokens");
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

        private void OnLoginSuccessHandler(LoginResult loginResult, string customId, CreateParams createParams)
        {
            Debug.Log("PlayFab login successful!");
            var isSuccessful = true;
            if (loginResult.NewlyCreated) // Create new account
            {
                if (createParams == null)
                {
                    Debug.LogError("Create new account with no info");
                    OnFatalError?.Invoke(loginResult);
                    isSuccessful = false;
                }
                else
                {
                    var account = new NodeAccount();
                    account.playFabId = loginResult.PlayFabId;
                    account.sessionTicket = loginResult.SessionTicket;
                    account.customId = customId;
                    account.serverID = createParams.server;
                    _clusterAccount.accounts.Add(account);

                    if (createParams.isCreateMaster)
                    {
                        // service master account
                        _clusterAccount.MasterId = loginResult.PlayFabId;
                        _clusterAccount.isMaster = true;
                        OnCreateMasterAccount?.Invoke(loginResult, createParams);
                    }
                    else
                    {
                        OnCreateNodeAccount?.Invoke(loginResult, createParams);
                    }
                }
            }
            else // Login to exist account
            {
                var curServerID = PFHelper.FindServerFromStatistic(loginResult.InfoResultPayload.PlayerStatistics);
                if (!string.IsNullOrEmpty(curServerID))
                {
                    var account = _clusterAccount.accounts.Find(a => a.playFabId.Equals(loginResult.PlayFabId));
                    if (account == null)
                    {
                        account = new NodeAccount();
                        _clusterAccount.accounts.Add(account);
                    }

                    account.playFabId = loginResult.PlayFabId;
                    account.sessionTicket = loginResult.SessionTicket;
                    account.customId = customId;
                    account.serverID = curServerID;
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
                _clusterAccount.startUpPlayFabId = loginResult.PlayFabId;
                SaveToLocal();
                OnLoginSuccess?.Invoke(loginResult, createParams);
                // Todo: Butin change current Session Ticket of PlayFab Sdk
            }
        }

        #endregion

        /// <summary>
        /// Call in login
        /// </summary>
        private void LoginFromCacheOrAutoSignUp()
        {
            if (_clusterAccount.accounts.Count > 0)
            {
                var startUpAcc = _clusterAccount.GetStartUpAccount();
                if (startUpAcc != null && !string.IsNullOrEmpty(startUpAcc.customId))
                {
                    // login to exist account
                    LoginWithCustomID(startUpAcc.customId);
                    return;
                }
            }

            // sign-up new account
            var customId = Guid.NewGuid().ToString();
            var suggestServer = "0"; // todo: set suggest Server
            LoginWithCustomID(customId, new CreateParams {isCreateMaster = true, server = suggestServer});
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
                    OnLoginSuccessHandler(res, customId, createParams);
                },
                err =>
                {
                    errorCallback?.Invoke(err);
                    OnRequestFailure?.Invoke(err);
                });
        }

        public void SwitchServer(string serverId)
        {
            var acc = _clusterAccount.FindAccountByServerId(serverId);
            if (acc != null)
            {
                LoginWithCustomID(acc.customId, null, result => { }, error => { });
            }
            else
            {
                LoginWithCustomID($"{serverId}-{Guid.NewGuid().ToString()}", null, result => { }, error => { });
            }
        }
    }
}
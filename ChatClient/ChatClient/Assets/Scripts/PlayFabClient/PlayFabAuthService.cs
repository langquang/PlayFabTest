﻿using System;
using Newtonsoft.Json;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.SharedModels;
using Share.PlayFabShare.Models;
using UnityEngine;

namespace PlayFabCustom
{
    public class PlayFabAuthService
    {
        private const string PREFS_TOKEN_KEY = "tokens"; // key in PlayerPrefs

        public Action<LoginResult, CreateParams> OnLoginSuccess;

        // public Action<LoginResult, CreateParams> OnCreateNodeAccount;
        // public Action<LoginResult, CreateParams> OnCreateMasterAccount;
        public Action<LoginResult> OnFatalError;

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
            // OnCreateMasterAccount += CreateMasterAccountHandler;
            // OnCreateNodeAccount += CreateNodeAccountHandler;
            LoadFromLocal();
            LoginFromCacheOrAutoSignUp();
        }

        public void Stop()
        {
            // OnCreateMasterAccount = CreateMasterAccountHandler;
            // OnCreateNodeAccount -= CreateNodeAccountHandler;

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
                        // OnCreateMasterAccount?.Invoke(loginResult, createParams);
                        createParams.needRegisterMasterAccount = true;
                    }
                    else
                    {
                        // OnCreateNodeAccount?.Invoke(loginResult, createParams);
                        createParams.needRegisterNodeAccount = true;
                    }
                }
            }
            else // Login to exist account
            {
                var curServerID = PFClientHelper.FindServerFromStatistic(loginResult.InfoResultPayload.PlayerStatistics);
                if (curServerID > 0)
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
            var suggestServer = 1; // todo: set suggest Server
            LoginWithCustomID(customId, new CreateParams {isCreateMaster = true, server = suggestServer});
        }

        public void LoginWithCustomID(string customId, CreateParams createParams = null, Action<LoginResult, CreateParams> OnSuccess = null, Action<PlayFabError> onError = null)
        {
            // Login with custom id on UNITY_EDITOR
            PlayFabClientAPI.LoginWithCustomID(new LoginWithCustomIDRequest
                {
                    CreateAccount = true,
                    CustomId = customId,
                    TitleId = PlayFabSettings.TitleId,
                    InfoRequestParameters = PFClientHelper.loginInfoRequestParams
                },
                res =>
                {
                    OnLoginSuccessHandler(res, customId, createParams);
                    OnSuccess?.Invoke(res, createParams);
                },
                err => { onError?.Invoke(err); });
        }

        public void SwitchServer(int serverId)
        {
            var acc = _clusterAccount.FindAccountByServerId(serverId);
            if (acc != null)
            {
                LoginWithCustomID(acc.customId, null, (result, createParams) => { }, error => { });
            }
            else
            {
                var customParams = new CreateParams
                {
                    server = serverId,
                    masterId = _clusterAccount.MasterId
                };

                LoginWithCustomID($"{serverId}-{Guid.NewGuid().ToString()}", customParams, (result, createParams) => { }, error => { });
            }
        }

        public string getMasterId()
        {
            return _clusterAccount.MasterId;
        }

        public bool IsContainsAccount(int server)
        {
            return _clusterAccount.FindAccountByServerId(server) != null;
        }
    }
}
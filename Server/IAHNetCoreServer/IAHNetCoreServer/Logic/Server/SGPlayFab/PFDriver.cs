using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IAHNetCoreServer.Logic.Server.Setting;
using IAHNetCoreServer.Logic.Server.SGPlayFab.Define;
using Newtonsoft.Json;
using PlayFab;
using PlayFab.ServerModels;
using PlayFabCustom.Models;
using PlayFabShare;
using SourceShare.Share.NetworkV2.Utils;

namespace IAHNetCoreServer.Logic.Server.SGPlayFab
{
    public class PFDriver
    {
        public static void Setup()
        {
            PlayFabSettings.staticSettings.TitleId = "20443";
            PlayFabSettings.staticSettings.DeveloperSecretKey = "U7XWD3YGJFIOD3HX7F74J75RYOOGE4UHO75KGMK7APBBQUPBUJ";
        }

        public static async Task<bool> CommitChangedToPlayFab(DataPlayer player)
        {
            var receipt = player.PrepareCommitChangedData();
            return await UpdateUserData(player, receipt);
        }

        #region GET DATA BY GROUP FUNCTION

        private static async Task<bool> LoadPublicData(DataPlayer player, int dataFlag)
        {
            var request = new GetUserDataRequest
            {
                PlayFabId = player.PlayerId,
                Keys = PFPlayerDataFlag.ConvertToUserDataNames(dataFlag)
            };
            var result = await PlayFabServerAPI.GetUserDataAsync(request);
            if (result.Error != null)
            {
                return false;
            }

            player.UpdateDataFromPayload(result.Result.Data);
            return true;
        }

        private static async Task<bool> LoadReadOnlyData(DataPlayer player, int dataFlag)
        {
            var request = new GetUserDataRequest
            {
                PlayFabId = player.PlayerId,
                Keys = PFPlayerDataFlag.ConvertToUserDataNames(dataFlag)
            };
            var result = await PlayFabServerAPI.GetUserReadOnlyDataAsync(request);
            if (result.Error != null)
            {
                return false;
            }

            player.UpdateDataFromPayload(result.Result.Data);
            return true;
        }

        private static async Task<bool> LoadInternalData(DataPlayer player, int dataFlag)
        {
            var request = new GetUserDataRequest
            {
                PlayFabId = player.PlayerId,
                Keys = PFPlayerDataFlag.ConvertToUserDataNames(dataFlag)
            };
            var result = await PlayFabServerAPI.GetUserInternalDataAsync(request);
            if (result.Error != null)
            {
                return false;
            }

            player.UpdateDataFromPayload(result.Result.Data);
            return true;
        }

        public static async Task<bool> SaveInternalData(DataPlayer player, int playFabDataFlag)
        {
            var data = player.ExportPlayerInternalData(playFabDataFlag);
            if (data.Count == 0)
            {
                return false;
            }

            var request = new UpdateUserInternalDataRequest()
            {
                PlayFabId = player.PlayerId,
                Data = data
            };
            var result = await PlayFabServerAPI.UpdateUserInternalDataAsync(request);
            if (result.Error != null)
            {
                return false;
            }

            return true;
        }

        #endregion


        /// <summary>
        ///
        /// todo: use cloud script?????
        /// </summary>
        /// <returns></returns>
        public static async Task<bool> LoadUserData(DataPlayer player, int dataFlag)
        {
            var tasks = new List<Task<bool>>(3);
            if (PFPlayerDataFlag.IsContainsInternalData(dataFlag))
            {
                tasks.Add(LoadInternalData(player, dataFlag));
            }

            if (PFPlayerDataFlag.IsContainsReadOnlyData(dataFlag))
            {
                tasks.Add(LoadReadOnlyData(player, dataFlag));
            }

            if (PFPlayerDataFlag.IsContainsPublicData(dataFlag))
            {
                tasks.Add(LoadPublicData(player, dataFlag));
            }

            if (tasks.Count <= 0) return false;
            
            var result = await Task.WhenAll(tasks);
            return result.Any(success => success);
        }

        /// <summary>
        ///  Load newest data from PlayFab and fill to player
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public static async Task<DataPlayer> LoadUserData(DataPlayer player)
        {
            var combinedInfo = new GetPlayerCombinedInfoRequestParams()
            {
                GetUserAccountInfo = true,
                GetPlayerStatistics = true,
                GetPlayerProfile = true,
                GetUserReadOnlyData = true,
                GetUserData = true,
                GetUserInventory = true,
                GetUserVirtualCurrency = true,
                ProfileConstraints = new PlayerProfileViewConstraints()
                {
                    ShowDisplayName = true,
                    ShowLocations = true,
                    ShowStatistics = true,
                }
            };

            var request = new GetPlayerCombinedInfoRequest()
            {
                PlayFabId = player.PlayerId,
                InfoRequestParameters = combinedInfo
            };
            var result = await PlayFabServerAPI.GetPlayerCombinedInfoAsync(request);
            if (result.Error != null)
            {
                Debugger.Write($"PFDrive: GetUserData fail {result.Error.ErrorMessage}");
                return null;
            }

            player.UpdateDataFromPayload(result.Result.InfoResultPayload);
            return player;
        }

        /// <summary>
        /// Core class Update UserData into PlayFab. It'll call ExecuteCloudScript "UpdateUserData"
        /// </summary>
        /// <param name="player">User data.</param>
        /// <param name="receipt">Data update required.</param>
        /// <param name="errCallback">Error callback.</param>
        private static async Task<bool> UpdateUserData(DataPlayer player, PFUpdatePlayerReceipt receipt, Action<string> errCallback = null)
        {
            var request = new ExecuteCloudScriptServerRequest
            {
                PlayFabId = player.PlayerId,
                FunctionName = PFCloudScripFuncName.CS_UPDATE_USER_DATA,
                FunctionParameter = JsonConvert.SerializeObject(receipt),
                GeneratePlayStreamEvent = true,
                RevisionSelection = GameSetting.DEFAULT_CLOUD_SCRIPT_VERSION_IS_LATEST ? CloudScriptRevisionOption.Latest : CloudScriptRevisionOption.Live
            };

            var result = await PlayFabServerAPI.ExecuteCloudScriptAsync(request);
            if (result.Error != null)
            {
                errCallback.Invoke(result.Error.ErrorMessage);
                // ToDo: Handler UpdateUserData fail
                return false;
            }

            Debugger.Write($"UpdateUserData {player.PlayerId} successful");
            return true;
        }

        public static Task<PlayFabResult<AuthenticateSessionTicketResult>> AuthenticateSessionTicketAsync(string sessionTicket)
        {
            return PlayFabServerAPI.AuthenticateSessionTicketAsync(new AuthenticateSessionTicketRequest() {SessionTicket = sessionTicket}); // asynchronous here, make a sub task, calling thread is free and comeback to next line of code when sub task is done
        }
    }
}
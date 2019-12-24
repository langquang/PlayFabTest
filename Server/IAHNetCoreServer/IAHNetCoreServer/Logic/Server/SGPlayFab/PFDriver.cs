using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IAHNetCoreServer.Logic.Server.Setting;
using IAHNetCoreServer.Logic.Server.SGPlayFab.Define;
using PlayFab;
using PlayFab.ServerModels;
using PlayFabCustom.Models;
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
        
        /// <summary>
        ///  Load newest data from PlayFab and fill to player
        /// </summary>
        /// <param name="player"></param>
        /// <param name="combinedInfo"></param>
        /// <returns></returns>
        public static async Task<DataPlayer> GetUserData(DataPlayer player, GetPlayerCombinedInfoRequestParams combinedInfo = null)
        {
            if (combinedInfo == null)
            {
                combinedInfo = new GetPlayerCombinedInfoRequestParams()
                {
                    GetUserAccountInfo = true,
                    GetPlayerStatistics = true,
                    GetPlayerProfile = true,
                    GetUserReadOnlyData = true,
                    GetUserInventory = true,
                    GetUserVirtualCurrency = true,
                    ProfileConstraints = new PlayerProfileViewConstraints()
                    {
                        ShowDisplayName = true,
                        ShowLocations = true,
                        ShowStatistics = true,
                    }
                };
            }

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
            else
            {
                player.UpdateDataFromPayload(result.Result.InfoResultPayload);
                return player;
            }
        }

        /// <summary>
        /// Core class Update UserData into PlayFab. It'll call ExecuteCloudScript "UpdateUserData"
        /// </summary>
        /// <param name="player">User data.</param>
        /// <param name="receipt">Data update required.</param>
        /// <param name="errCallback">Error callback.</param>
        public static async Task UpdateUserData(DataPlayer player, PFUpdatePlayerReceipt receipt, Action<string> errCallback = null)
        {
            var request = new ExecuteCloudScriptServerRequest()
            {
                PlayFabId = player.PlayerId,
                FunctionName = PFCloudScripFuncName.CS_UPDATE_USER_DATA,
                FunctionParameter = receipt,
                GeneratePlayStreamEvent = true,
                RevisionSelection = GameSetting.DEFAULT_CLOUD_SCRIPT_VERSION_IS_LATEST ? CloudScriptRevisionOption.Latest : CloudScriptRevisionOption.Live
            };

            var result = await PlayFabServerAPI.ExecuteCloudScriptAsync(request);
            if (result.Error != null)
            {
                errCallback.Invoke(result.Error.ErrorMessage);
                // ToDo: Handler UpdateUserData fail
            }
            else
            {
                Debugger.Write($"UpdateUserData {player.PlayerId} successful");
            }
        }
        
        public static Task<PlayFabResult<AuthenticateSessionTicketResult>> AuthenticateSessionTicketAsync(string sessionTicket)
        {
            return PlayFabServerAPI.AuthenticateSessionTicketAsync(new AuthenticateSessionTicketRequest() {SessionTicket = sessionTicket}); // asynchronous here, make a sub task, calling thread is free and comeback to next line of code when sub task is done
        }

        public static Task<PlayFabResult<GetUserDataResult>> GetInternalData(DataPlayer player, List<string> keys)
        {
            var request = new GetUserDataRequest()
            {
                PlayFabId = player.PlayerId,
                Keys = keys
            };
            return PlayFabServerAPI.GetUserInternalDataAsync(request);
        }

        public static async Task<bool> SaveInternalData(DataPlayer player, int playFabDataFlag)
        {
            var data = player.ExportData(playFabDataFlag);
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
    }
}
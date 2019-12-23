using System.Collections.Generic;
using System.Threading.Tasks;
using PlayFab;
using PlayFab.ServerModels;
using PlayFabCustom.Models;
using SourceShare.Share.PlayFabCustom;

namespace PlayFabCustom
{
    public class PlayFabService
    {
        #region SINGLETON

        public static PlayFabService Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new PlayFabService();
                }

                return _instance;
            }
        }

        private static PlayFabService _instance;

        public PlayFabService()
        {
            _instance = this;
        }

        #endregion

        public static void Setup()
        {
            PlayFabSettings.staticSettings.TitleId = "20443";
            PlayFabSettings.staticSettings.DeveloperSecretKey = "U7XWD3YGJFIOD3HX7F74J75RYOOGE4UHO75KGMK7APBBQUPBUJ";
        }

        public static Task<PlayFabResult<AuthenticateSessionTicketResult>> AuthenticateSessionTicketAsync(string sessionTicket)
        {
            return PlayFabServerAPI.AuthenticateSessionTicketAsync(new AuthenticateSessionTicketRequest() {SessionTicket = sessionTicket}); // asynchronous here, make a sub task, calling thread is free and comeback to next line of code when sub task is done
        }

        public static Task<PlayFabResult<GetUserDataResult>> LoadInternalData(DataPlayer player, List<string> keys)
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
using System.Collections.Generic;
using PlayFab.ClientModels;
using PlayFabShare.Models;

namespace PlayFabCustom
{
    public class PFHelper
    {
        public static readonly GetPlayerCombinedInfoRequestParams loginInfoRequestParams = new GetPlayerCombinedInfoRequestParams
        {
            GetUserAccountInfo = true,
            GetUserInventory = true,
            GetPlayerProfile = true,
            GetPlayerStatistics = true,
            GetUserVirtualCurrency = true,
            GetTitleData = true,
            GetUserReadOnlyData = true,
            GetUserData = true,
            ProfileConstraints = new PlayerProfileViewConstraints
            {
                ShowDisplayName = true,
                ShowLocations = true,
                ShowStatistics = true,
            }
        };

        public static int FindServerFromStatistic(List<StatisticValue> payload)
        {
            var statistic = payload.Find(s => s.StatisticName.Equals(PFStatistic.SERVER));
            return statistic?.Value ?? 0;
        }
    }

    public class CreateParams
    {
        // create params
        public bool isCreateMaster;
        public int  server;
        public string masterId;
        // action params
        public bool needRegisterMasterAccount;
        public bool needRegisterNodeAccount;
    }
}
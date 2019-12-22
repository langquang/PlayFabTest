using System.Collections.Generic;
using PlayFab.ClientModels;
using PlayFabCustom.Models;

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

        public static string FindServerFromStatistic(List<StatisticValue> payload)
        {
            var statistic = payload.Find(s => s.StatisticName.Equals(PlayerStatistic.SERVER));
            return statistic == null ? string.Empty : $"{statistic.Value}";
        }
    }

    public class CreateParams
    {
        public bool   isCreateMaster;
        public string server;
    }
}
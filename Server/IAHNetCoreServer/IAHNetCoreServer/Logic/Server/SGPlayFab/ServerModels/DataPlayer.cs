using System.Collections.Generic;
using LiteNetLib;
using Newtonsoft.Json;
using PlayFab.ServerModels;
using PlayFabShare;
using PlayFabShare.Models;
using SourceShare.Share.NetworkV2;
using SourceShare.Share.NetworkV2.Router;


namespace PlayFabCustom.Models
{
    public class DataPlayer : NetPlayer
    {
        public bool IsLoadedPlayFabData { get; set; }
        
        public string PFCustomId { get; private set; }

        public PFProfile Profile { get; set; }
        public PFStatistic Statistic { get; set; }
        
        public ClusterAccount ClusterAccount { get; set; }

        public DataPlayer(string playerId, NetPeer peer, NetRouter router, bool isClient, string token) : base(playerId, peer, router, isClient, token)
        {
            Profile = new PFProfile();
            Statistic = new PFStatistic();
            ClusterAccount = new ClusterAccount();
        }

        public bool IsMasterAccount()
        {
            return ClusterAccount.isMaster;
        }

        public bool IsNodeAccount()
        {
            return !ClusterAccount.isMaster && !string.IsNullOrEmpty(ClusterAccount.MasterId) && !PlayerId.Equals(ClusterAccount.MasterId);
        }

        public void UpdateDataFromPayload(GetPlayerCombinedInfoResultPayload payload)
        {
            // Try to get customId
            if (payload.AccountInfo != null && payload.AccountInfo.CustomIdInfo != null)
            {
                PFCustomId = payload.AccountInfo.CustomIdInfo.CustomId;
            }
            // Import Profile
            Profile.Import(payload.PlayerProfile);
            // Import Statistic
            Statistic.Import(payload.PlayerStatistics);
            // Import User Data
            UpdateDataFromPayload(payload.UserData);
        }

        public void UpdateDataFromPayload(Dictionary<string, UserDataRecord> payload)
        {
            if (payload != null)
            {
                payload.TryGetValue(PFPlayerDataKey.ACCOUNT, out var account);
                if (account != null && !string.IsNullOrEmpty(account.Value))
                {
                    ClusterAccount = JsonConvert.DeserializeObject<ClusterAccount>(account.Value);
                }
            }
        }
        
        public Dictionary<string, string> ExportData(int playFabDataFlag)
        {
            var exportData = new Dictionary<string, string>();
            if ((playFabDataFlag & PFPlayerDataFlag.ACCOUNT) != 0)
            {
                exportData.Add(PFPlayerDataKey.ACCOUNT, JsonConvert.SerializeObject(ClusterAccount));
            }

            return exportData;
        }
    }
}
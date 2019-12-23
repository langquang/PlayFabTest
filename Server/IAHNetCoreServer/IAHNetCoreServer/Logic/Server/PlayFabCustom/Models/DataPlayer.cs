using System.Collections.Generic;
using LiteNetLib;
using Newtonsoft.Json;
using PlayFab.ServerModels;
using SourceShare.Share.NetworkV2;
using SourceShare.Share.NetworkV2.Router;
using SourceShare.Share.PlayFabCustom;

namespace PlayFabCustom.Models
{
    public class DataPlayer : NetPlayer
    {
        public string CustomId { get; private set; }

        public PlayerProfile Profile { get; set; }
        public PlayerStatistic Statistic { get; set; }
        public ClusterAccount ClusterAccount { get; set; }

        public DataPlayer(string playerId, NetPeer peer, NetRouter router, bool isClient, string token, string customId) : base(playerId, peer, router, isClient, token)
        {
            CustomId = customId;
        }

        public bool IsMasterAccount()
        {
            return ClusterAccount.isMaster;
        }

        public bool IsNodeAccount()
        {
            return !ClusterAccount.isMaster && !string.IsNullOrEmpty(ClusterAccount.MasterId) && !PlayerId.Equals(ClusterAccount.MasterId);
        }

        public void UpdateDataFromPayload(Dictionary<string, UserDataRecord> payload)
        {
            payload.TryGetValue(PFPlayerDataKey.ACCOUNT, out var account);
            if (account != null && !string.IsNullOrEmpty(account.Value))
            {
                ClusterAccount = JsonConvert.DeserializeObject<ClusterAccount>(account.Value);
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
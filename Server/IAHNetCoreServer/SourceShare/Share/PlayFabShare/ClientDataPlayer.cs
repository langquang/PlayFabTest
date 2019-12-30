using System.Collections.Generic;
using LiteNetLib;
using Newtonsoft.Json;
using PlayFabShare.Models;
using SourceShare.Share.NetworkV2;
#if SERVER_SIDE
using PlayFab.ServerModels;
#else
using PlayFab.ClientModels;
#endif

namespace PlayFabShare
{
    public class ClientDataPlayer : NetPlayer
    {
        public bool IsOnline { get; private set; }

    #region PLAYFAB DATA PROPS

        public PFProfile Profile { get; set; }
        public PFStatistic Statistic { get; set; }

        private PFCurrency Currency { get; set; }

        private PFInventory Inventory { get; set; }
        public ClusterAccount ClusterAccount { get; set; }

        public KeyReward KeyReward { get; set; }

    #endregion

        public ClientDataPlayer(string playerId, NetPeer peer, string token) : base(playerId, peer, token)
        {
            IsOnline       = peer != null;
            Profile        = new PFProfile();
            Statistic      = new PFStatistic();
            Currency       = new PFCurrency();
            Inventory      = new PFInventory();
            ClusterAccount = new ClusterAccount();
            // entities
            KeyReward = new KeyReward();
        }

        public ClientDataPlayer(string playerId) : base(playerId, null, null)
        {
            IsOnline = false;
        }

    #region PARSING DATA FROM PLAYFAB

        public void UpdateDataFromPayload(GetPlayerCombinedInfoResultPayload payload)
        {
            // Import Profile
            Profile.Import(payload.PlayerProfile);
            // Import Statistic
            Statistic.Import(payload.PlayerStatistics);
            // Import Inventory
            Inventory.Import(payload.UserInventory);
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

    #endregion
    }
}
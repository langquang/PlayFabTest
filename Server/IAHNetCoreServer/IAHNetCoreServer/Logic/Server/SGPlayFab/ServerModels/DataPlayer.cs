using System.Collections.Generic;
using IAHNetCoreServer.Logic.Server.SGPlayFab;
using LiteNetLib;
using Newtonsoft.Json;
using PlayFab.ServerModels;
using PlayFabShare;
using PlayFabShare.Models;
using SourceShare.Share.NetworkV2;
using SourceShare.Share.NetworkV2.Utils;
using SourceShare.Share.Utils;

namespace PlayFabCustom.Models
{
    public class DataPlayer : NetPlayer
    {
        public bool IsLoadedPlayFabData { get; set; }

        public string PFCustomId { get; private set; }
        
        public bool IsOnline { get; private set; }

        #region PLAYFAB DATA PROPS

        public PFProfile Profile { get; set; }
        public PFStatistic Statistic { get; set; }

        public PFCurrency Currency { get; set; }

        public ClusterAccount ClusterAccount { get; set; }

        #endregion

        #region AUTO TRACKING CHANGE DATA PROPS

        private PFUpdatePlayerReceipt _updateReceipt = new PFUpdatePlayerReceipt();
        
        #endregion

        public DataPlayer(string playerId, NetPeer peer, string token) : base(playerId, peer, token)
        {
            IsOnline = true;
            Profile = new PFProfile();
            Statistic = new PFStatistic();
            Currency = new PFCurrency();
            ClusterAccount = new ClusterAccount();
        }

        public DataPlayer(string playerId): base(playerId, null, null)
        {
            IsOnline = false;
        }

        #region ACCOUNT TYPE

        public bool IsMasterAccount()
        {
            return ClusterAccount.isMaster;
        }

        public bool IsNodeAccount()
        {
            return !ClusterAccount.isMaster && !string.IsNullOrEmpty(ClusterAccount.MasterId) && !PlayerId.Equals(ClusterAccount.MasterId);
        }

        #endregion

        #region PARSING DATA FROM PLAYFAB

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

        #endregion

        #region TEMP DIRECT DATA FUNCTIONS

        public Dictionary<string, string> ExportPlayerInternalData(int playFabDataFlag)
        {
            Dictionary<string, string> exportData = null;
            if ((playFabDataFlag & PFPlayerDataFlag.ACCOUNT) != 0)
            {
                if (exportData == null)
                    exportData = new Dictionary<string, string>();
                exportData.Add(PFPlayerDataKey.ACCOUNT, JsonConvert.SerializeObject(ClusterAccount));
            }

            return exportData;
        }

        #endregion

        #region AUTO TRACKING CHANGE DATA FUNCTIONS

        public int Server
        {
            get => Statistic.Server;

            set
            {
                if (value <= 0)
                {
                    Debugger.WriteError($"<{PlayerId}> Set Server <= 0");
                }
                else
                {
                    Statistic.Server = value;
                    _updateReceipt.UpdateStatistics(PFStatistic.SERVER, value);
                }
            }
        }

        public int Level
        {
            get => Statistic.Level;

            set
            {
                if (value <= 0)
                {
                    Debugger.WriteError($"<{PlayerId}> Set level <= 0");
                }
                else
                {
                    Statistic.Level = value;
                    _updateReceipt.UpdateStatistics(PFStatistic.LEVEL, value);
                }
            }
        }

        public void IncreaseGem(int incValue)
        {
            if (incValue <= 0)
            {
                Debugger.WriteError($"<{PlayerId}> IncreaseGem <= 0");
                return;
            }

            Currency.Gem = MathHelper.SafeIncreaseIntValue(Currency.Gem, incValue);
            _updateReceipt.IncreaseCurrency(PFCurrency.GEM, incValue);
        }


        public void DecreaseGem(int decValue)
        {
            if (decValue <= 0)
            {
                Debugger.WriteError($"<{PlayerId}> DecreaseGem <= 0");
                return;
            }

            Currency.Gem = MathHelper.SafeDecreaseIntValue(Currency.Gem, decValue);
            _updateReceipt.DecreaseCurrency(PFCurrency.GEM, decValue);
        }

        public void AddChangedDataFlag(int flag)
        {
            _updateReceipt.AddChangedDataFlag(flag);
        }

        public PFUpdatePlayerReceipt PrepareToCommitChangedData()
        {
            var receipt = _updateReceipt;
            _updateReceipt = new PFUpdatePlayerReceipt(); // make new receipt for next change

            // Export Player Data (Title)
            int flag = receipt.TotalChangedDataFlag;
            if (flag == 0) // not change
            {
                return receipt;
            }

            var internalData = ExportPlayerInternalData(flag);
            if (internalData != null)
            {
                receipt.UpdateInternalData(internalData);
            }

            return receipt;
        }

        #endregion
    }
}
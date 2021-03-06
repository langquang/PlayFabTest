using System.Collections.Generic;
using IAHNetCoreServer.Logic.Server.Setting;
using IAHNetCoreServer.Logic.Server.SGPlayFab.CustomModels;
using Newtonsoft.Json;
using PlayFab.ServerModels;
using SourceShare.Share.NetworkV2.Utils;
using SourceShare.Share.Utils;


namespace IAHNetCoreServer.Logic.Server.SGPlayFab
{
    /// <summary>
    /// Core Class contains all data required to update into PlayFab.
    /// </summary>
    public class PFUpdatePlayerReceipt
    {
        // Secret key
        [JsonProperty("key")] private readonly string _key;

        //----------------------------------------------------------------------
        //  Virtual Currencies
        //----------------------------------------------------------------------
        [JsonProperty("CurrencyReward")]   private Dictionary<string, int> _currencyReward;   // currency code -> gain value
        [JsonProperty("CurrencyDecrease")] private Dictionary<string, int> _currencyDecrease; // currency code -> decrease value

        //----------------------------------------------------------------------
        //  Inventory
        //----------------------------------------------------------------------
        private                                   string                      _catalogVersion; //add version to determine what's version using update in catalog
        [JsonProperty("ItemsGrant")]      private List<string>                _itemsGrant;// todo support grant item with custom data via api GrantItemsToUsers, just add a new property
        [JsonProperty("ItemRevoke")]      private List<RevokeInventoryItem>   _itemRevoke;
        [JsonProperty("ItemsCustomData")] private List<ItemUpdateCustomData>  _itemsCustomData;
        [JsonProperty("ItemsModifyUses")] private List<ModifyItemUsesRequest> _itemsModifyUses;

        //----------------------------------------------------------------------
        //  Statistic
        //----------------------------------------------------------------------
        [JsonProperty("Statistic")] private List<StatisticUpdate> _statistic;

        //----------------------------------------------------------------------
        //  Player Data (Title) - Internal
        //----------------------------------------------------------------------
        [JsonProperty("InternalData")] private Dictionary<string, string> _internalData;

        //----------------------------------------------------------------------
        //  Player Data (Title) - Read Only
        //----------------------------------------------------------------------
        [JsonProperty("ReadOnlyData")] private Dictionary<string, string> _readOnlyData;

        private int _totalChangedDataFlag;

    #region PROPERTIES

        [JsonIgnore] public int TotalChangedDataFlag => _totalChangedDataFlag;

    #endregion

        public PFUpdatePlayerReceipt()
        {
            _key = GameSetting.CLOUD_SCRIPT_SECRET_KEY;
        }


        //----------------------------------------------------------------------
        //  HELPER METHODS
        //----------------------------------------------------------------------
        public void IncreaseCurrency(string currencyName, int incValue)
        {
            if (_currencyReward == null)
                _currencyReward = new Dictionary<string, int>();

            var beginValue = 0;
            if (_currencyReward.TryGetValue(currencyName, out var currentReward))
            {
                beginValue = currentReward;
            }

            int afterValue = MathHelper.SafeIncreaseIntValue(beginValue, incValue);
#if DEBUG_AUTO_CHANGE_PF_DATA
            Debugger.Write($"[DEBUG_AUTO_CHANGE_PF_DATA] {currencyName} change from {beginValue} to {afterValue}");
#endif
            _currencyReward.Add(currencyName, afterValue);
        }

        public void DecreaseCurrency(string currencyName, int decValue)
        {
            if (_currencyDecrease == null)
                _currencyDecrease = new Dictionary<string, int>();

            var beginValue = 0;
            if (_currencyDecrease.TryGetValue(currencyName, out var currentDecrease))
            {
                beginValue = currentDecrease;
            }

            int afterValue = MathHelper.SafeIncreaseIntValue(beginValue, decValue); // Note increase <decrease num> here
#if DEBUG_AUTO_CHANGE_PF_DATA
            Debugger.Write($"[DEBUG_AUTO_CHANGE_PF_DATA] {currencyName} change from {beginValue} to {afterValue}");
#endif
            _currencyDecrease.Add(currencyName, afterValue);
        }

        public void UpdateStatistics(string statName, int value)
        {
            if (_statistic == null)
                _statistic = new List<StatisticUpdate>();

            var stat = _statistic.Find(r => r.StatisticName.Equals(statName));
            if (stat != null)
            {
                stat.Value = value;
            }
            else
            {
                _statistic.Add(new StatisticUpdate()
                               {
                                   StatisticName = statName,
                                   Value         = value
                               });
            }
        }

        public void UpdateReadOnlyData(string key, string value)
        {
            if (_readOnlyData == null)
                _readOnlyData = new Dictionary<string, string>();
            _readOnlyData[key] = value;
        }


        public void UpdateInternalData(string key, string value)
        {
            if (_internalData == null)
                _internalData = new Dictionary<string, string>();
            _internalData[key] = value;
        }

        public void UpdateInternalData(Dictionary<string, string> data)
        {
            if (_internalData == null)
                _internalData = new Dictionary<string, string>();

            foreach (var keyValuePair in data)
            {
                _internalData[keyValuePair.Key] = keyValuePair.Value;
            }
        }

        public void GrantNewItem(string itemId)
        {
            if (_itemsGrant == null)
                _itemsGrant = new List<string>();

            _itemsGrant.Add(itemId);
        }

        public void RevokeItem(RevokeInventoryItem item)
        {
            if (_itemRevoke == null)
                _itemRevoke = new List<RevokeInventoryItem>();
            _itemRevoke.Add(item);
        }

        public void ModifyExistItemUses(ModifyItemUsesRequest modifyItemUsesRequest)
        {
            if (_itemsModifyUses == null)
                _itemsModifyUses = new List<ModifyItemUsesRequest>();
            _itemsModifyUses.Add(modifyItemUsesRequest);
        }

        public void UpdateExistItemCustomData(ItemInstance itemInstance)
        {
            if (_itemsCustomData == null)
                _itemsCustomData = new List<ItemUpdateCustomData>();
            _itemsCustomData.Add(new ItemUpdateCustomData {itemInstance = itemInstance});
        }

        public void AddChangedDataFlag(int flag)
        {
            _totalChangedDataFlag |= flag;
        }
    }
}
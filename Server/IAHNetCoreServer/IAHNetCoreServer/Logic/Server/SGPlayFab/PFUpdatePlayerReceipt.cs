using System.Collections.Generic;
using IAHNetCoreServer.Logic.Server.SGPlayFab.CustomModels;
using PlayFab.ServerModels;

namespace IAHNetCoreServer.Logic.Server.SGPlayFab
{
    /// <summary>
    /// Core Class contains all data required to update into PlayFab.
    /// </summary>
    public class PFUpdatePlayerReceipt
    {
        //----------------------------------------------------------------------
        //  Virtual Currencies
        //----------------------------------------------------------------------
        private Dictionary<string, uint> _currencyReward;   // currency code -> gain value
        private Dictionary<string, uint> _currencyDecrease; // currency code -> decrease value

        //----------------------------------------------------------------------
        //  Inventory
        //----------------------------------------------------------------------
        private string                      _catalogVersion; //add version to determine what's version using update in catalog
        private List<ItemGrant>             _itemsGrant;
        private List<RevokeInventoryItem>   _itemRevoke;
        private List<ItemUpdateCustomData>  _itemsCustomData;
        private List<ModifyItemUsesRequest> _itemsModifyUses;

        //----------------------------------------------------------------------
        //  Statistic
        //----------------------------------------------------------------------
        private List<StatisticUpdate> _statistic;

        //----------------------------------------------------------------------
        //  Player Data (Title) - Internal
        //----------------------------------------------------------------------
        private Dictionary<string, string> _internalData;

        //----------------------------------------------------------------------
        //  Player Data (Title) - Read Only
        //----------------------------------------------------------------------
        private Dictionary<string, string> _readOnlyData;


        #region PROPERTIES

        public Dictionary<string, uint> CurrencyReward => _currencyReward;

        public Dictionary<string, uint> CurrencyDecrease => _currencyDecrease;

        public string CatalogVersion => _catalogVersion;

        public List<ItemGrant> ItemsGrant => _itemsGrant;

        public List<RevokeInventoryItem> ItemRevoke => _itemRevoke;

        public List<ItemUpdateCustomData> ItemsCustomData => _itemsCustomData;

        public List<ModifyItemUsesRequest> ItemsModifyUses => _itemsModifyUses;

        public List<StatisticUpdate> Statistic => _statistic;

        public Dictionary<string, string> InternalData => _internalData;

        public Dictionary<string, string> ReadOnlyData => _readOnlyData;

        #endregion


        //----------------------------------------------------------------------
        //  HELPER METHODS
        //----------------------------------------------------------------------
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
                    Value = value
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

        public ItemGrant GrantNewInventoryItem(string playFabId, string itemId)
        {
            if (_itemsGrant == null)
                _itemsGrant = new List<ItemGrant>();

            var item = new ItemGrant {PlayFabId = playFabId, ItemId = itemId};
            _itemsGrant.Add(item);
            return item;
        }

        public void UpdateExistInventoryItem(ItemInstance itemInstance)
        {
            if (_itemsCustomData == null)
                _itemsCustomData = new List<ItemUpdateCustomData>();
            _itemsCustomData.Add(new ItemUpdateCustomData {itemInstance = itemInstance});
        }
    }
}
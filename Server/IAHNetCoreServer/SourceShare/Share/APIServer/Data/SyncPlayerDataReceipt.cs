using System.Collections.Generic;
using MessagePack;
using Newtonsoft.Json;
#if SERVER_SIDE
using PlayFab.ServerModels;

#endif

namespace SourceShare.Share.APIServer.Data
{
    [MessagePackObject]
    public class SyncPlayerDataReceipt
    {
        //  Virtual Currencies
        [Key(0)] public Dictionary<string, int> Currency; // currency code -> value

        //  Statistic
        [Key(1)] public Dictionary<string, int> Statistic;

        //  Read Only Data, Internal Data
        [Key(2)] public Dictionary<int, string> JsonEntities; // SyncDataName -> json

        //  Inventory
        [Key(3)] public List<string> JsonUpdateItems; // grant or update inventory item
        [Key(4)] public List<string> RevokeItems;     // delete inventory item

    #region SERVER_CODE

#if SERVER_SIDE
        [IgnoreMember] private Dictionary<int, IEntity> _entities { get; set; } // SyncDataName -> json
        [IgnoreMember] private Dictionary<string, ItemInstance> _updateItems;

#endif

    #endregion

        public SyncPlayerDataReceipt()
        {
        }


    #region SERVER_CODE

#if SERVER_SIDE
        public bool IsEmpty()
        {
            return Currency == null && Statistic == null && RevokeItems == null && _updateItems == null && _entities == null;
        }

        public void SyncCurrency(string currencyCode, int afterValue)
        {
            if (Currency == null)
                Currency = new Dictionary<string, int>();
            Currency[currencyCode] = afterValue;
        }

        public void SyncStatistic(string name, int value)
        {
            if (Statistic == null)
                Statistic = new Dictionary<string, int>();
            Statistic[name] = value;
        }

        public void SyncUpdateItem(ItemInstance item)
        {
            if (_updateItems == null)
                _updateItems = new Dictionary<string, ItemInstance>();
            _updateItems[item.ItemInstanceId] = item;
        }

        public void SyncEntities(int name, IEntity entity)
        {
            if (_entities == null)
                _entities = new Dictionary<int, IEntity>();
            _entities[name] = entity;
        }

        public void SyncRevokeItem(ItemInstance item)
        {
            if (RevokeItems == null)
                RevokeItems = new List<string>();
            if (!RevokeItems.Contains(item.ItemInstanceId))
                RevokeItems.Add(item.ItemInstanceId);
        }

        public void SerializeJson()
        {
            // data
            if (_entities != null && _entities.Count > 0)
            {
                JsonEntities = new Dictionary<int, string>(_entities.Count);
                foreach (var pair in _entities)
                {
                    JsonEntities.Add(pair.Key, JsonConvert.SerializeObject(pair.Value));
                }
            }

            // item
            if (_updateItems != null && _updateItems.Count > 0)
            {
                JsonUpdateItems = new List<string>(_updateItems.Count);
                foreach (var pair in _updateItems)
                {
                    JsonUpdateItems.Add(JsonConvert.SerializeObject(pair.Value));
                }
            }
        }
#endif

    #endregion
    }
}
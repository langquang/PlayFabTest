using System.Collections.Generic;
using MessagePack;
using Newtonsoft.Json;
#if SERVER_SIDE
using PlayFab.ServerModels;

#else
using PlayFab.ClientModels;
#endif

namespace SourceShare.Share.APIServer.Data
{
    [MessagePackObject]
    public class SyncPlayerDataReceipt
    {
        //----------------------------------------------------------------------
        //  Virtual Currencies
        //----------------------------------------------------------------------
        [Key(0)] public Dictionary<string, int> Currency { get; set; } // currency code -> value

        //----------------------------------------------------------------------
        //  Statistic
        //----------------------------------------------------------------------
        [Key(1)] public Dictionary<string, int> Statistic { get; set; }

        //----------------------------------------------------------------------
        //  Data
        //----------------------------------------------------------------------
        [Key(2)] public Dictionary<int, string> JsonEntities { get; set; } // SyncDataName -> json

// #if SERVER_SIDE
        [IgnoreMember] private Dictionary<int, IEntity> _entities { get; set; } // SyncDataName -> json
// #endif

        //----------------------------------------------------------------------
        //  Reward
        //----------------------------------------------------------------------
        [Key(3)] public List<string> JsonUpdateItems { get; set; } // grant or update item

// #if SERVER_SIDE
        [IgnoreMember] private Dictionary<string, ItemInstance> _updateItems;
// #endif


        //----------------------------------------------------------------------
        //  Revoke Item
        //----------------------------------------------------------------------
        [Key(4)] public List<string> RevokeItems { get; set; } // delete item

        public SyncPlayerDataReceipt()
        {
            Currency    = new Dictionary<string, int>();
            Statistic   = new Dictionary<string, int>();
            RevokeItems = new List<string>();

        #region SERVER_CODE

#if SERVER_SIDE
            _updateItems = new Dictionary<string, ItemInstance>();
            _entities    = new Dictionary<int, IEntity>();
#endif

        #endregion
        }


    #region SERVER_CODE

// #if SERVER_SIDE
        public void SyncCurrency(string currencyCode, int afterValue)
        {
            Currency[currencyCode] = afterValue;
        }

        public void SyncStatistic(string name, int value)
        {
            Statistic[name] = value;
        }

        public void SyncUpdateItem(ItemInstance item)
        {
            _updateItems[item.ItemInstanceId] = item;
        }

        public void SyncEntities(int name, IEntity entity)
        {
            _entities[name] = entity;
        }

        public void SyncRevokeItem(ItemInstance item)
        {
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
// #endif

    #endregion

    #region CLIENT_CODE

    #endregion
    }
}
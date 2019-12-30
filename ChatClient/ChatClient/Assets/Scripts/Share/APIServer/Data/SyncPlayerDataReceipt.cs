using System.Collections.Generic;
using MessagePack;
#if SERVER_SIDE
using PlayFab.ServerModels;
#endif

namespace SourceShare.Share.APIServer.Data
{
    [MessagePackObject]
    public class SyncPlayerDataReceipt
    {
        //----------------------------------------------------------------------
        //  Virtual Currencies
        //----------------------------------------------------------------------
        [Key(0)] private Dictionary<string, int> _currency; // currency code -> value

        //----------------------------------------------------------------------
        //  Statistic
        //----------------------------------------------------------------------
        [Key(1)] private Dictionary<string, int> _statistic;

        //----------------------------------------------------------------------
        //  Data
        //----------------------------------------------------------------------
        [Key(2)] private Dictionary<int, string> _jsonEntities; // SyncDataName -> json
#if SERVER_SIDE
        [IgnoreMember] private Dictionary<int, IEntity> _entities; // SyncDataName -> json
#endif

        //----------------------------------------------------------------------
        //  Reward
        //----------------------------------------------------------------------
        [Key(3)] private List<string> _jsonUpdateItems; // grant or update item
#if SERVER_SIDE
        [IgnoreMember] private List<ItemInstance> _updateItems;
#endif


        //----------------------------------------------------------------------
        //  Revoke Item
        //----------------------------------------------------------------------
        [Key(4)] private List<string> _revokeItems; // delete item

        public SyncPlayerDataReceipt()
        {
            _currency    = new Dictionary<string, int>();
            _statistic   = new Dictionary<string, int>();
            _revokeItems = new List<string>();

        #region SERVER_CODE

#if SERVER_SIDE
            _updateItems = new List<ItemInstance>();
            _entities    = new Dictionary<int, IEntity>();
#endif

        #endregion
        }


    #region SERVER_CODE

#if SERVER_SIDE
        public void SyncCurrency(string currencyCode, int afterValue)
        {
            _currency[currencyCode] = afterValue;
        }

        public void SyncStatistic(string name, int value)
        {
            _currency[name] = value;
        }

        public void UpdateItem(ItemInstance item)
        {
            _updateItems.Add(item);
        }

        public void SyncEntities(int name, IEntity entity)
        {
            _entities[name] = entity;
        }

        public void RevokeItem(string itemInstanceId)
        {
            _revokeItems.Add(itemInstanceId);
        }
#endif

    #endregion
    }
}
using System.Collections.Generic;
using MessagePack;

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
        [Key(4)] private Dictionary<int, IEntity> _entities; // SyncDataName -> json

        //----------------------------------------------------------------------
        //  Reward
        //----------------------------------------------------------------------
        [Key(2)] private Dictionary<string, int> _currencyReward; // currency code -> gain value
        [Key(3)] private List<TempInventoryItem> _grantItems;


        //----------------------------------------------------------------------
        //  Revoke Item
        //----------------------------------------------------------------------
        [Key(4)] private List<string> _revokeItems;

        public void SyncCurrency(string currencyCode, int rewardValue, int afterValue)
        {
            if (_currency == null)
            {
                _currency = new Dictionary<string, int>();
                _currencyReward = new Dictionary<string, int>();
            }

            _currency[currencyCode] = afterValue;
            _currencyReward[currencyCode] = rewardValue;
        }

        public void SyncStatistic(string name, int value)
        {
            if (_statistic == null)
            {
                _statistic = new Dictionary<string, int>();
            }

            _currency[name] = value;
        }

        public void GrantItem(TempInventoryItem item)
        {
            if (_grantItems == null)
            {
                _grantItems = new List<TempInventoryItem>();
            }

            _grantItems.Add(item);
        }

        public void SyncEntities(int name, IEntity entity)
        {
            if (_entities == null)
            {
                _entities = new Dictionary<int, IEntity>();
            }

            _entities[name] = entity;
        }

        public void RevokeItem(string itemInstanceId)
        {
            if (_revokeItems == null)
            {
                _revokeItems = new List<string>();
            }

            _revokeItems.Add(itemInstanceId);
        }
    }
}
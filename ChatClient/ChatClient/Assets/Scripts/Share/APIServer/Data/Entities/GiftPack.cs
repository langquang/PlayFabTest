using System.Collections.Generic;
using MessagePack;

namespace SourceShare.Share.APIServer.Data
{
    [MessagePackObject]
    public class GiftPack : IEntity
    {
        //----------------------------------------------------------------------
        //  Virtual Currencies
        //----------------------------------------------------------------------
        [Key(0)] private Dictionary<string, int> _primitiveInts; // currency code, statistic name -> value
        
        //----------------------------------------------------------------------
        //  Items
        //----------------------------------------------------------------------
        [Key(1)] private List<string> _items; // itemId

        [IgnoreMember]
        public Dictionary<string, int> PrimitiveInts => _primitiveInts;

        [IgnoreMember]
        public List<string> Items => _items;

        public GiftPack()
        {
            _primitiveInts = new Dictionary<string, int>();
            _items = new List<string>();
        }
        
        public void IncPrimitiveInt(string name, int incValue)
        {
            if (_primitiveInts.ContainsKey(name))
                _primitiveInts[name] += incValue;
            else
                _primitiveInts[name] = incValue;
        }

        public void AddItem(string itemId)
        {
            _items.Add(itemId);
        }
    }
}
using System.Collections.Generic;
using MessagePack;

namespace Share.APIServer.Data.Entities
{
    [MessagePackObject]
    public class GiftPack : IEntity
    {
        //----------------------------------------------------------------------
        //  Virtual Currencies, Statistic
        //----------------------------------------------------------------------
        [Key(0)] public Dictionary<string, int> PrimitiveInts; // currency code, statistic name -> value

        //----------------------------------------------------------------------
        //  Items
        //----------------------------------------------------------------------
        [Key(1)] public List<string> Items; // itemId

        public GiftPack()
        {
        }

        public void IncPrimitiveInt(string name, int incValue)
        {
            if (PrimitiveInts == null)
                PrimitiveInts = new Dictionary<string, int>();

            if (PrimitiveInts.ContainsKey(name))
                PrimitiveInts[name] += incValue;
            else
                PrimitiveInts[name] = incValue;
        }

        public void AddItem(string itemId)
        {
            if (Items == null)
                Items = new List<string>();

            Items.Add(itemId);
        }
    }
}
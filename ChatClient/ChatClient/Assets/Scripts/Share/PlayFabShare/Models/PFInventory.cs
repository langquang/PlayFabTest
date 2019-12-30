
using System.Collections.Generic;
using System.Linq;
#if SERVER_SIDE
using PlayFab.ServerModels;
#else
using PlayFab.ClientModels;
#endif

namespace PlayFabShare.Models
{
    public class PFInventory
    {
        private readonly Dictionary<string, ItemInstance> _items; // itemInstanceId -> instance

        public PFInventory()
        {
            _items = new Dictionary<string, ItemInstance>();
        }

        public ItemInstance FindFromInstanceId(string itemInstanceId)
        {
            _items.TryGetValue(itemInstanceId, out var itemInstance);
            return itemInstance;
        }

        public IEnumerable<ItemInstance> FindFromItemId(string itemId)
        {
            var match = _items
                .Where(pair => pair.Value.ItemId.Equals(itemId))
                .Select(pair => pair.Value);
            return match;
        }

        public void Grant(ItemInstance instance)
        {
            _items.Add(instance.ItemInstanceId, instance);
        }
        
        /// <summary>
        ///  Override or make new
        /// </summary>
        /// <param name="instance"></param>
        public void Set(ItemInstance instance)
        {
            _items[instance.ItemInstanceId] = instance; // override
        }

        public void Revoke(ItemInstance instance)
        {
            _items.Remove(instance.ItemInstanceId);
        }
        
        public void Import(List<ItemInstance> UserInventory)
        {
           _items.Clear();
           UserInventory.ForEach(i=>_items.Add(i.ItemInstanceId, i));
        }
    }
}
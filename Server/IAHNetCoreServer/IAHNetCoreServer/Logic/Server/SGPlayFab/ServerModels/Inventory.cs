using System.Collections.Generic;
using System.Linq;
using PlayFab.ServerModels;

namespace IAHNetCoreServer.Logic.Server.SGPlayFab.ServerModels
{
    public class Inventory
    {
        private readonly Dictionary<string, ItemInstance> _items; // itemInstanceId -> instance

        public Inventory()
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

        public void Grant(string itemId)
        {
            Grant(new ItemInstance(){ItemInstanceId = });
        }
    }
}
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using PlayFabShare.Models.Base;
using SourceShare.Share.APIServer.Data;
using SourceShare.Share.NetworkV2.Utils;
#if SERVER_SIDE
using PlayFab.ServerModels;

#else
using PlayFab.ClientModels;
#endif

namespace PlayFabShare.Models
{
    public class PFInventory : ISyncEntity
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

        public void Revoke(List<string> listInstanceId)
        {
            foreach (var instanceId in listInstanceId)
            {
                _items.Remove(instanceId);
            }
        }

        public void Import(List<ItemInstance> UserInventory)
        {
            _items.Clear();
            UserInventory.ForEach(i => _items.Add(i.ItemInstanceId, i));
        }

        public void Sync(SyncPlayerDataReceipt syncReceipt)
        {
#if DEBUG_SYNC_DATA
            Debugger.Write(syncReceipt.RevokeItems, "RevokeItems");
            Debugger.Write(syncReceipt.JsonUpdateItems, "UpdateItems");
#endif
            if (syncReceipt.RevokeItems != null)
            {
                Revoke(syncReceipt.RevokeItems);
            }

            if (syncReceipt.JsonUpdateItems != null)
            {
                foreach (var jsonUpdateItem in syncReceipt.JsonUpdateItems)
                {
                    var item = JsonConvert.DeserializeObject<ItemInstance>(jsonUpdateItem);
                    Set(item);
                }
            }
        }
    }
}
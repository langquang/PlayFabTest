using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IAHNetCoreServer.Logic.Server.Setting;
using NLog;
using PlayFab;
using PlayFab.ServerModels;

namespace IAHNetCoreServer.Logic.Server.SGPlayFab
{
    public class PFCatalog
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private static readonly HashSet<string> StackableItem = new HashSet<string>();

        public static async Task Init()
        {
            var request = new GetCatalogItemsRequest {CatalogVersion = GameSetting.CURRENT_CATALOG_VERSION};
            var result = await PlayFabServerAPI.GetCatalogItemsAsync(request);
            if (result == null || result.Error != null)
            {
                throw new Exception("Can not load PlayFab Catalog");
            }

            Logger.Info($"Get PlayFab Catalog successful, numOfItem={result.Result.Catalog.Count}");
            foreach (var catalogItem in result.Result.Catalog)
            {
                if (catalogItem.IsStackable)
                {
                    StackableItem.Add(catalogItem.ItemId);
                }
            }
        }

        public static bool IsStackable(string itemId)
        {
            return StackableItem.Contains(itemId);
        }
    }
}
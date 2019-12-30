using PlayFab.ServerModels;

namespace IAHNetCoreServer.Logic.Server.SGPlayFab
{
    public class PFHelper
    {
        public static ItemInstance Convert(GrantedItemInstance instance)
        {
            return new ItemInstance
                   {
                       Annotation = instance.Annotation,
                       BundleContents = instance.BundleContents,
                       BundleParent = instance.BundleParent,
                       CatalogVersion = instance.CatalogVersion,
                       CustomData = instance.CustomData,
                       DisplayName = instance.DisplayName,
                       Expiration = instance.Expiration,
                       ItemClass = instance.ItemClass,
                       ItemId = instance.ItemId,
                       ItemInstanceId = instance.ItemInstanceId,
                       PurchaseDate = instance.PurchaseDate,
                       RemainingUses = instance.RemainingUses,
                       UnitCurrency = instance.UnitCurrency,
                       UnitPrice = instance.UnitPrice,
                       UsesIncrementedBy = instance.UsesIncrementedBy
                   };
        }
    }
}
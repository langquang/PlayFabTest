using Share.APIServer.Data;

namespace Share.PlayFabShare.Models.Base
{
    public interface ISyncEntity
    {
        void Sync(SyncPlayerDataReceipt syncReceipt);
    }
}
using SourceShare.Share.APIServer.Data;

namespace PlayFabShare.Models.Base
{
    public interface ISyncEntity
    {
        void Sync(SyncPlayerDataReceipt syncReceipt);
    }
}
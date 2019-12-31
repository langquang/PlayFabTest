using System.Threading.Tasks;
using PlayFabCustom.Models;
using SourceShare.Share.NetRequest;

namespace SourceShare.Share.APIServer.Data
{
    public class SyncHelper
    {
        public static void SyncDataToClient(DataPlayer player, SyncPlayerDataReceipt receipt = null)
        {
            SyncPlayerDataReceipt sentReceipt;
            if (receipt == null)
            {
                sentReceipt = player.RollSyncReceipt(); // use internal receipt
            }
            else
            {
                sentReceipt = receipt;
            }
            sentReceipt.SerializeJson();
            var syncMessage = new SyncDataMessage(sentReceipt);
            player.Send(syncMessage);
        }
    }
}
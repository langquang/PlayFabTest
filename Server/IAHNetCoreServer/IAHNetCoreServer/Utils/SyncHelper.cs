using System.Threading.Tasks;
using PlayFabCustom.Models;
using SourceShare.Share.NetRequest;

namespace SourceShare.Share.APIServer.Data
{
    public class SyncHelper
    {
        public static void SyncDataToClient(DataPlayer player)
        {
            var receipt = player.PrepareSyncData();
            var syncMessage = new SyncDataMessage(receipt);
            player.Send(syncMessage);
        }
    }
}
using Newtonsoft.Json;
using PlayFabShare.Models.Base;
using SourceShare.Share.APIServer.Data;
using SourceShare.Share.NetworkV2.Utils;

namespace PlayFabShare.Models
{
    public class PFData : ISyncEntity
    {
        public KeyReward KeyReward { get; set; }

        public PFData()
        {
            KeyReward = new KeyReward();
        }

        public void Sync(SyncPlayerDataReceipt syncReceipt)
        {
#if DEBUG_SYNC_DATA
            Debugger.Write("Sync PFData");
            Debugger.Write(syncReceipt.JsonEntities, "JsonEntities", Debugger.FindConstName<SyncEntityName>);
#endif
            if (syncReceipt.JsonEntities != null)
            {
                foreach (var pair in syncReceipt.JsonEntities)
                {
                    switch (pair.Key)
                    {
                        case SyncEntityName.ONLINE_REWARD:
                            KeyReward.OnlineReward = JsonConvert.DeserializeObject<OnlineReward>(pair.Value);
                            break;
                        default:
                            Debugger.WriteError($"Unknown entity name: {pair.Key}, value={pair.Value}");
                            break;
                    }
                }
            }
        }
    }
}
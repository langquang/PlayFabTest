using System.Collections.Generic;
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
    public class PFStatistic : ISyncEntity
    {
        public const string SERVER = "server";
        public const string LEVEL  = "level";

        public int Server { get; set; }
        public int Level { get; set; }

        public PFStatistic()
        {
        }

        public void Import(List<StatisticValue> payload)
        {
            foreach (var element in payload)
            {
                switch (element.StatisticName)
                {
                    case SERVER:
                        Server = element.Value;
                        break;
                    case LEVEL:
                        Level = element.Value;
                        break;
                }
            }
        }

        public void Import(Dictionary<string, int> payload)
        {
#if DEBUG_SYNC_DATA
            Debugger.Write(payload, "PFStatistic");
#endif
            foreach (var pair in payload)
            {
                switch (pair.Key)
                {
                    case SERVER:
                        Server = pair.Value;
                        break;
                    case LEVEL:
                        Level = pair.Value;
                        break;
                }
            }
        }

        public void Sync(SyncPlayerDataReceipt syncReceipt)
        {
            if (syncReceipt.Statistic != null)
            {
                Import(syncReceipt.Statistic);
            }
        }
    }
}
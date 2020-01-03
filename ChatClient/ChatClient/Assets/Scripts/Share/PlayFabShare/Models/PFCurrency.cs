using System.Collections.Generic;
using Share.APIServer.Data;
using Share.NetworkV2.Utils;
using Share.PlayFabShare.Models.Base;

namespace Share.PlayFabShare.Models
{
    public class PFCurrency : ISyncEntity
    {
        public const string GEM  = "GE";
        public const string GOLD = "GO";

        public int Gem { get; set; }
        public int Gold { get; set; }

        public void Import(Dictionary<string, int> payload)
        {
#if DEBUG_SYNC_DATA
        Debugger.Write(payload, "PFCurrency");
#endif
            foreach (var pair in payload)
            {
                switch (pair.Key)
                {
                    case GEM:
                        Gem = pair.Value;
                        break;
                    case GOLD:
                        Gold = pair.Value;
                        break;
                }
            }
        }

        public void Sync(SyncPlayerDataReceipt syncReceipt)
        {
            if (syncReceipt.Currency != null)
            {
                Import(syncReceipt.Currency);
            }
        }
    }
}
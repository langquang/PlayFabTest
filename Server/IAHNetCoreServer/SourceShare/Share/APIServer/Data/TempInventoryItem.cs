using System.Collections.Generic;
using MessagePack;

namespace SourceShare.Share.APIServer.Data
{
    [MessagePackObject]
    public class TempInventoryItem
    {
        [Key(0)] public string                     ItemId;
        [Key(1)] public string                     ItemInstanceId;
        [Key(2)] public int                        RemainingUses;
        [Key(3)] public Dictionary<string, string> CustomData;

        public TempInventoryItem(string itemId, string itemInstanceId, int remainingUses = 1)
        {
            ItemId = itemId;
            ItemInstanceId = itemInstanceId;
            RemainingUses = remainingUses;
        }
    }
}
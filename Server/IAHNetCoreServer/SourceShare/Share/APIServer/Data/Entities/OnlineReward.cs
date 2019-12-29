using System;
using MessagePack;

namespace SourceShare.Share.APIServer.Data
{
    [MessagePackObject]
    public class OnlineReward : IEntity
    {
        [Key(0)]
        public DateTime nextAt;
        [Key(1)]
        public bool isClaimed;
    }
}
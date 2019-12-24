using LiteNetLib.Utils;
using MessagePack;
using SourceShare.Share.NetworkV2.TransportData.Base;

namespace SourceShare.Share.NetRequest
{
    [MessagePackObject]
    public class CreateNodeAccountRequest : INetData
    {
        [Key(0)] public int serverId;
        [Key(1)] public int masterId;

        public override void Serialize(NetDataWriter writer)
        {
            base.Serialize(writer);
            writer.PutBytesWithLength(MessagePackSerializer.Serialize(this));
        }
    }

    [MessagePackObject()]
    public class CreateNodeAccountResponse : INetData
    {
        public override void Serialize(NetDataWriter writer)
        {
            base.Serialize(writer);
            writer.PutBytesWithLength(MessagePackSerializer.Serialize(this));
        }
    }
}
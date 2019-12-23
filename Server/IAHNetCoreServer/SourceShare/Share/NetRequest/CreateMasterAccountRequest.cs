using LiteNetLib.Utils;
using MessagePack;
using SourceShare.Share.NetworkV2.TransportData.Base;

namespace SourceShare.Share.NetRequest
{
    [MessagePackObject]
    public class CreateMasterAccountRequest : INetData
    {
        [Key(0)] public int serverId;
        public override void Serialize(NetDataWriter writer)
        {
            base.Serialize(writer);
            writer.PutBytesWithLength(MessagePackSerializer.Serialize(this));
        }
    }

    [MessagePackObject()]
    public class CreateMasterAccountResponse : INetData
    {
        public override void Serialize(NetDataWriter writer)
        {
            base.Serialize(writer);
            writer.PutBytesWithLength(MessagePackSerializer.Serialize(this));
        }
    }
}
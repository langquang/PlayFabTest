using LiteNetLib.Utils;
using MessagePack;
using SourceShare.Share.NetRequest.Config;
using SourceShare.Share.NetworkV2.TransportData.Base;
using SourceShare.Share.NetworkV2.TransportData.Define;
using SourceShare.Share.NetworkV2.TransportData.Header;

namespace SourceShare.Share.NetRequest
{
    [MessagePackObject]
    public class CreateNodeAccountRequest : INetData
    {
        [Key(0)] public int serverId;
        [Key(1)] public string masterId;

        public CreateNodeAccountRequest(int serverId, string masterId) : base(new RequestHeader(ENetType.REQUEST, NetAPICommand.CREATE_NODE_ACCOUNT))
        {
            this.serverId = serverId;
            this.masterId = masterId;
        }

        public override void Serialize(NetDataWriter writer)
        {
            base.Serialize(writer);
            writer.PutBytesWithLength(MessagePackSerializer.Serialize(this));
        }
    }

    [MessagePackObject]
    public class CreateNodeAccountResponse : INetData
    {
        public override void Serialize(NetDataWriter writer)
        {
            base.Serialize(writer);
            writer.PutBytesWithLength(MessagePackSerializer.Serialize(this));
        }
    }
    
    [MessagePackObject]
    public class CheckCreateNodeAccountRequest : INetData
    {
        [Key(0)] public int    serverId;
        [Key(1)] public string masterId;

        public CheckCreateNodeAccountRequest(int serverId, string masterId) : base(new RequestHeader(ENetType.REQUEST, NetAPICommand.CHECK_CREATE_NODE_ACCOUNT))
        {
            this.serverId = serverId;
            this.masterId = masterId;
        }

        public override void Serialize(NetDataWriter writer)
        {
            base.Serialize(writer);
            writer.PutBytesWithLength(MessagePackSerializer.Serialize(this));
        }
    }

    [MessagePackObject]
    public class CheckCreateNodeAccountResponse : INetData
    {
        public override void Serialize(NetDataWriter writer)
        {
            base.Serialize(writer);
            writer.PutBytesWithLength(MessagePackSerializer.Serialize(this));
        }
    }
}
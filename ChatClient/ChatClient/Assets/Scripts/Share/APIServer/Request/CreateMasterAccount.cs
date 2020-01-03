using MessagePack;
using Share.APIServer.Request.Config;
using LiteNetLib.Utils;
using Share.NetworkV2.TransportData.Base;
using Share.NetworkV2.TransportData.Define;
using Share.NetworkV2.TransportData.Header;

namespace Share.APIServer.Request
{
    [MessagePackObject]
    public class CreateMasterAccountRequest : INetData
    {
        [Key(0)] public int serverId;

        public CreateMasterAccountRequest(int serverId) : base(new RequestHeader(ENetType.REQUEST, NetAPICommand.CREATE_MASTER_ACCOUNT))
        {
            this.serverId = serverId;
        }

        public override void Serialize(NetDataWriter writer)
        {
            base.Serialize(writer);
            writer.PutBytesWithLength(MessagePackSerializer.Serialize(this));
        }
    }

    [MessagePackObject]
    public class CreateMasterAccountResponse : INetData
    {
        [Key(0)] public int serverId;
        
        public override void Serialize(NetDataWriter writer)
        {
            base.Serialize(writer);
            writer.PutBytesWithLength(MessagePackSerializer.Serialize(this));
        }
    }
}
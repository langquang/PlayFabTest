using MessagePack;
using Share.APIServer.Data;
using Share.APIServer.Request.Config;
using LiteNetLib.Utils;
using Share.NetworkV2.TransportData.Base;
using Share.NetworkV2.TransportData.Define;
using Share.NetworkV2.TransportData.Header;

namespace Share.APIServer.Request
{
    [MessagePackObject]
    public class SyncDataMessage : INetData
    {
        [Key(0)] public SyncPlayerDataReceipt Receipt;

        public SyncDataMessage() // require constructor in MessagePack
        {
        }

        public SyncDataMessage(SyncPlayerDataReceipt Receipt) : base(new RequestHeader(ENetType.REQUEST, NetAPICommand.SYNC_DATA))
        {
            this.Receipt = Receipt;
        }

        public override bool IsValid()
        {
            return Receipt != null;
        }

        public override void Serialize(NetDataWriter writer)
        {
            base.Serialize(writer);
            writer.PutBytesWithLength(MessagePackSerializer.Serialize(this));
        }
    }
}
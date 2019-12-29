using LiteNetLib.Utils;
using MessagePack;
using SourceShare.Share.APIServer.Data;
using SourceShare.Share.NetRequest.Config;
using SourceShare.Share.NetworkV2.TransportData.Base;
using SourceShare.Share.NetworkV2.TransportData.Define;
using SourceShare.Share.NetworkV2.TransportData.Header;

namespace SourceShare.Share.NetRequest
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
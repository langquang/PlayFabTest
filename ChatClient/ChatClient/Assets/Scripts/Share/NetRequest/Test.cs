using LiteNetLib.Utils;
using MessagePack;
using SourceShare.Share.TransportData.Base;
using SourceShare.Share.TransportData.define;
using SourceShare.Share.TransportData.Define;
using SourceShare.Share.TransportData.Header;

namespace SourceShare.Share.TransportData
{
    [MessagePackObject]
    public class TestRequest : INetData
    {
        [Key(0)] public string msg;

        public TestRequest()
        {
        }

        public TestRequest(string msg) : base(new RequestHeader(ENetType.REQUEST, NetAPICommand.TEST_REQUEST))
        {
            this.msg = msg;
        }

        public override bool IsValid()
        {
            return !string.IsNullOrEmpty(msg);
        }

        public override void Serialize(NetDataWriter writer)
        {
            base.Serialize(writer);
            writer.PutBytesWithLength(MessagePackSerializer.Serialize(this));
        }
    }

    [MessagePackObject]
    public class TestResponse : INetData
    {
        [Key(0)] public string msg;

        public TestResponse()
        {
        }

        public TestResponse(INetData request, int errorCode = 0)
        {
            Header = new ResponseHeader(request.Header);
        }

        public override void Serialize(NetDataWriter writer)
        {
            base.Serialize(writer);
            writer.PutBytesWithLength(MessagePackSerializer.Serialize(this));
        }
    }
}
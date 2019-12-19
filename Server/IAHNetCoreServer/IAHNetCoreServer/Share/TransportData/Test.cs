using IAHNetCoreServer.Share.TransportData.Base;
using IAHNetCoreServer.Share.TransportData.Define;
using IAHNetCoreServer.Share.TransportData.Header;
using LiteNetLib.Utils;
using MessagePack;

namespace IAHNetCoreServer.Share.TransportData
{
    [MessagePackObject]
    public class TestRequest : Request
    {
        [Key(0)] public string msg;

        public TestRequest()
        {
        }

        public TestRequest(string msg) : base(new RequestHeader(ENetType.REQUEST, ENetCommand.TEST_REQUEST))
        {
            this.msg = msg;
        }

        // public LoginRequest(RequestHeader header) : base(header)
        // {
        // }

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
    public class TestResponse : Response
    {
        [Key(0)] public string msg;
        
        public TestResponse(Request request, int errorCode = 0) : base(request, errorCode)
        {
        }

        public override void Serialize(NetDataWriter writer)
        {
            base.Serialize(writer);
            writer.PutBytesWithLength(MessagePackSerializer.Serialize(this));
        }
    }
}
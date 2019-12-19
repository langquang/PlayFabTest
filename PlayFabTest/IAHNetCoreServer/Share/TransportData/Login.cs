using IAHNetCoreServer.Share.TransportData.Base;
using IAHNetCoreServer.Share.TransportData.Header;
using LiteNetLib.Utils;
using MessagePack;

namespace IAHNetCoreServer.Share.TransportData
{
    [MessagePackObject]
    public class LoginRequest : Request
    {
        [Key(0)] private string playerId;
        [Key(1)] private string sessionTicket;

        public LoginRequest()
        {
        }

        public LoginRequest(RequestHeader header) : base(header)
        {
        }

        public override bool IsValid()
        {
            return string.IsNullOrEmpty(playerId) && string.IsNullOrEmpty(sessionTicket);
        }

        public override void Serialize(NetDataWriter writer)
        {
            base.Serialize(writer);
            writer.PutBytesWithLength(MessagePackSerializer.Serialize(this));
        }
    }

    [MessagePackObject]
    public class LoginResponse : Response
    {
        [Key(0)] public string playerId;
        [Key(1)] public string sessionTicket;

        public LoginResponse(ResponseHeader header) : base(header)
        {
        }

        public override void Serialize(NetDataWriter writer)
        {
            base.Serialize(writer);
            writer.PutBytesWithLength(MessagePackSerializer.Serialize(this));
        }
    }

    [MessagePackObject]
    public class CommonErrorResponse : Response
    {
        public CommonErrorResponse(ResponseHeader header) : base(header)
        {
        }
    }
}
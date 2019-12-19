using IAHNetCoreServer.Share.TransportData.Base;
using IAHNetCoreServer.Share.TransportData.Define;
using IAHNetCoreServer.Share.TransportData.Header;
using LiteNetLib.Utils;
using MessagePack;

namespace IAHNetCoreServer.Share.TransportData
{
    [MessagePackObject]
    public class LoginRequest : Request
    {
        [Key(0)] public string playerId;
        [Key(1)] public string sessionTicket;

        public LoginRequest()
        {
        }

        public LoginRequest(string playerId, string sessionTicket) : base(new RequestHeader(ENetType.REQUEST, ENetCommand.LOGIN_REQUEST))
        {
            this.playerId = playerId;
            this.sessionTicket = sessionTicket;
        }

        // public LoginRequest(RequestHeader header) : base(header)
        // {
        // }

        public override bool IsValid()
        {
            return !string.IsNullOrEmpty(playerId) && !string.IsNullOrEmpty(sessionTicket);
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

        public LoginResponse()
        {
        }

        public LoginResponse(ResponseHeader header) : base(header)
        {
        }

        public LoginResponse(Request request, int errorCode) : base(request, errorCode)
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
        public CommonErrorResponse()
        {
        }

        public CommonErrorResponse(Request request, int errorCode) : base(request, errorCode)
        {
        }
    }
}
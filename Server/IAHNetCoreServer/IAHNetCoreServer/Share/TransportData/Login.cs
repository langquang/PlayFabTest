using IAHNetCoreServer.Share.TransportData.Base;
using IAHNetCoreServer.Share.TransportData.Define;
using IAHNetCoreServer.Share.TransportData.Header;
using LiteNetLib.Utils;
using MessagePack;

namespace IAHNetCoreServer.Share.TransportData
{
    [MessagePackObject]
    public class LoginRequest : INetData
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
    public class LoginResponse : INetData
    {
        [Key(0)] public string playerId;
        [Key(1)] public string sessionTicket;

        public LoginResponse()
        {
        }

        public LoginResponse(INetDataHeader header) : base(header)
        {
        }

        public override void Serialize(NetDataWriter writer)
        {
            base.Serialize(writer);
            writer.PutBytesWithLength(MessagePackSerializer.Serialize(this));
        }
    }

    [MessagePackObject]
    public class CommonErrorResponse : INetData
    {
        public CommonErrorResponse()
        {
        }

        public CommonErrorResponse(INetData request, int errorCode)
        {
            
        }
    }
}
using MessagePack;
using Share.APIServer.Request.Config;
using LiteNetLib.Utils;
using Share.NetworkV2.TransportData.Base;
using Share.NetworkV2.TransportData.Define;
using Share.NetworkV2.TransportData.Header;

namespace Share.APIServer.Request
{
    [MessagePackObject]
    public class LoginRequest : INetData
    {
        [Key(0)] public string playerId;
        [Key(1)] public string sessionTicket;

        public LoginRequest()
        {
        }

        public LoginRequest(string playerId, string sessionTicket) : base(new RequestHeader(ENetType.REQUEST, NetAPICommand.LOGIN))
        {
            this.playerId = playerId;
            this.sessionTicket = sessionTicket;
        }

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
        [Key(0)] public string token;

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
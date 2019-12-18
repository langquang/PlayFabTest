using IAHNetCoreServer.NetData.Header;
using IAHNetCoreServer.Server;
using LiteNetLib.Utils;
using MessagePack;

namespace IAHNetCoreServer.NetData
{
    [MessagePackObject]
    public class LoginRequest : Request
    {
        [Key(0)] private string _playerId;
        [Key(1)] private string _sessionTicket;

        public LoginRequest(RequestHeader header) : base(header)
        {
        }

        public override bool IsValid()
        {
            return string.IsNullOrEmpty(_playerId) && string.IsNullOrEmpty(_sessionTicket);
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
        [Key(0)] private string _playerId;
        [Key(1)] private string _sessionTicket;

        public LoginResponse(ResponseHeader header) : base(header)
        {
        }

        public override void Serialize(NetDataWriter writer)
        {
            base.Serialize(writer);
            writer.PutBytesWithLength(MessagePackSerializer.Serialize(this));
        }
    }
}
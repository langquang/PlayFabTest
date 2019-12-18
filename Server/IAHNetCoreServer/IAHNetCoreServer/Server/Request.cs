using IAHNetCoreServer.NetData.Header;
using LiteNetLib.Utils;

namespace IAHNetCoreServer.Server
{
    public abstract class Request : INetData
    {
        private RequestHeader _header;

        protected Request(RequestHeader header)
        {
            _header = header;
        }

        public abstract bool IsValid();
        
        public virtual void Serialize(NetDataWriter writer)
        {
            _header.Serialize(writer);
        }
    }
    
    public abstract class Response : INetData
    {
        private ResponseHeader _header;

        protected Response(ResponseHeader header)
        {
            _header = header;
        }

        public virtual void Serialize(NetDataWriter writer)
        {
            _header.Serialize(writer);
        }
    }
}
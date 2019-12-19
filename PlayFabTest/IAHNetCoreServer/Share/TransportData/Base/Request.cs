using IAHNetCoreServer.Share.TransportData.Header;
using LiteNetLib.Utils;

namespace IAHNetCoreServer.Share.TransportData.Base
{
    public abstract class Request : INetData
    {
        private readonly RequestHeader _header;

        protected Request()
        {
        }

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
}
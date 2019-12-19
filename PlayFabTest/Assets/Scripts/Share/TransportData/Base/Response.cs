using IAHNetCoreServer.Share.TransportData.Header;
using LiteNetLib.Utils;

namespace IAHNetCoreServer.Share.TransportData.Base
{
    public abstract class Response : INetData
    {
        private readonly ResponseHeader _header;

        protected Response()
        {
        }

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
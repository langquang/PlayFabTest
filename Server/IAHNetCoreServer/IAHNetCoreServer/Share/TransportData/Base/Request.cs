using IAHNetCoreServer.Share.TransportData.Header;
using LiteNetLib.Utils;

namespace IAHNetCoreServer.Share.TransportData.Base
{
    public abstract class Request<THeader> : INetData where THeader : INetDataHeader
    {
        public THeader Header { get; set; }

        protected Request()
        {
        }

        protected Request(THeader header)
        {
            Header = header;
        }

        public abstract bool IsValid();

        public virtual void Serialize(NetDataWriter writer)
        {
            Header.Serialize(writer);
        }
    }
}
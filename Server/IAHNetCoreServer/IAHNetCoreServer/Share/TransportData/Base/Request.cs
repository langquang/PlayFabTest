using IAHNetCoreServer.Share.TransportData.Header;
using LiteNetLib.Utils;
using MessagePack;

namespace IAHNetCoreServer.Share.TransportData.Base
{
    public abstract class Request : INetData
    {
        [IgnoreMember]
        public RequestHeader Header { get; set; }

        protected Request()
        {
        }

        protected Request(RequestHeader header)
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
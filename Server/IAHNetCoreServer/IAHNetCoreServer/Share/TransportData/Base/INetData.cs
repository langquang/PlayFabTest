using IAHNetCoreServer.Share.Router;
using IAHNetCoreServer.Share.TransportData.Header;
using LiteNetLib.Utils;
using MessagePack;

namespace IAHNetCoreServer.Share.TransportData.Base
{
    public abstract class INetData
    {
        [IgnoreMember]
        public INetDataHeader Header { get; set; }

        protected INetData()
        {
        }

        protected INetData(INetDataHeader header)
        {
            Header = header;
        }

        public virtual void Serialize(NetDataWriter writer)
        {
            writer.Put(HashName.GetHash(Header.GetType()));
            Header.Serialize(writer);
        }

        public virtual bool IsValid()
        {
            return true;
        }
    }
}
using MessagePack;
using LiteNetLib.Utils;
using Share.NetworkV2.TransportData.Header;
using Share.NetworkV2.Utils;

namespace Share.NetworkV2.TransportData.Base
{
    public abstract class INetData
    {
        protected INetData()
        {
        }

        protected INetData(INetDataHeader header)
        {
            Header = header;
        }

        [IgnoreMember] public INetDataHeader Header { get; set; }

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
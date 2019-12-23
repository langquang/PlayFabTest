using LiteNetLib.Utils;
using MessagePack;
using SourceShare.Share.NetworkV2.Router;
using SourceShare.Share.NetworkV2.TransportData.Header;

namespace SourceShare.Share.NetworkV2.TransportData.Base
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
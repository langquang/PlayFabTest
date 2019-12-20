using IAHNetCoreServer.Share.TransportData.Header;
using LiteNetLib.Utils;
using MessagePack;

namespace IAHNetCoreServer.Share.TransportData.Base
{
    public interface INetData<T> where T : INetDataHeader
    {
        T Header { get; set; }

        void Serialize(NetDataWriter writer);
    }
}
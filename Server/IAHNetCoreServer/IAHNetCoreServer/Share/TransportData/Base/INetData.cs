using IAHNetCoreServer.Share.TransportData.Header;
using MessagePack;

namespace IAHNetCoreServer.Share.TransportData.Base
{
    public interface INetData<T> where T : INetDataHeader
    {
        T Header { get; set; }
    }
}
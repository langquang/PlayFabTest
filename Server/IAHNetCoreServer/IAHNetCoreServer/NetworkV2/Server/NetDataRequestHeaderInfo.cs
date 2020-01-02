using SourceShare.Share.NetworkV2.TransportData.Define;

namespace IAHNetCoreServer.NetworkV2.Server
{
    public class NetDataRequestHeaderInfo
    {
        public ENetType netType;
        public int      netCommand;
        public string   token;

        public NetDataRequestHeaderInfo(ENetType netType, int netCommand, string token)
        {
            this.netType = netType;
            this.netCommand = netCommand;
            this.token = token;
        }
    }
}
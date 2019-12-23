using SourceShare.Share.TransportData.Define;

namespace IAHNetCoreServer.Server
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
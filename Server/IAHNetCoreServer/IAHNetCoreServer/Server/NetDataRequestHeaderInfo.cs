namespace IAHNetCoreServer.Server
{
    public class NetDataRequestHeaderInfo
    {
        public ENetType    netType;
        public ENetCommand netCommand;
        public string      token;

        public NetDataRequestHeaderInfo(ENetType netType, ENetCommand netCommand, string token)
        {
            this.netType = netType;
            this.netCommand = netCommand;
            this.token = token;
        }
    }
}
using LiteNetLib.Utils;

namespace IAHNetCoreServer.Server
{
    public class Request : INetData
    {
        private ENetCommand _command;
        
        public ENetType GetNetType()
        {
            return ENetType.REQUEST;
        }

        public ENetCommand GetNetCommand()
        {
            return _command;
        }

        public void Write(NetDataWriter writer)
        {
            throw new System.NotImplementedException();
        }
    }
}
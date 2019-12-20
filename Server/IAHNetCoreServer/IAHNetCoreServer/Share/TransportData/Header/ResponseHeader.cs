using IAHNetCoreServer.Share.TransportData.Define;
using LiteNetLib.Utils;

namespace IAHNetCoreServer.Share.TransportData.Header
{
    public class ResponseHeader : INetDataHeader
    {
        public int RequestId { get; set; }
        public int Error { get; set; }
        public ENetType NetType { get; set; }
        public ENetCommand NetCommand { get; set; }

        public ResponseHeader()
        {
        }

        public ResponseHeader(RequestHeader requestHeader, int error = 0)
        {
            NetType = requestHeader.NetType;
            NetCommand = requestHeader.NetCommand;
            RequestId = requestHeader.RequestId;
            Error = error;
        }

        // Solve as a Message
        public ResponseHeader(ENetType netType, ENetCommand netCommand, int error)
        {
            NetType = netType;
            NetCommand = netCommand;
            Error = error;
        }
        
        public void Deserialize(NetDataReader reader)
        {
            NetType = (ENetType) reader.GetInt();
            NetCommand = (ENetCommand) reader.GetInt();
            RequestId = reader.GetInt();
            Error = reader.GetInt();
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put((int) NetType);
            writer.Put((int) NetCommand);
            writer.Put(RequestId);
            writer.Put(Error);
        }
    }
}
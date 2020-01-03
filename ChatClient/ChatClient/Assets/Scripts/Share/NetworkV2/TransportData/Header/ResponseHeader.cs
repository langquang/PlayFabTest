using LiteNetLib.Utils;
using Share.NetworkV2.TransportData.Define;

namespace Share.NetworkV2.TransportData.Header
{
    public class ResponseHeader : INetDataHeader
    {
        public ResponseHeader()
        {
        }

        public ResponseHeader(INetDataHeader requestHeader, int error = 0)
        {
            NetType = ENetType.RESPONSE;
            NetCommand = requestHeader.NetCommand;
            RequestId = requestHeader.RequestId;
            Error = error;
        }

        // Solve as a Message
        public ResponseHeader(ENetType netType, int netCommand, int error)
        {
            NetType = netType;
            NetCommand = netCommand;
            Error = error;
        }

        public int Error { get; set; }
        public int RequestId { get; set; }
        public ENetType NetType { get; set; }
        public int NetCommand { get; set; }

        public void Deserialize(NetDataReader reader)
        {
            NetType = (ENetType) reader.GetInt();
            NetCommand = reader.GetInt();
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
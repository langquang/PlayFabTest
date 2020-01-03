using LiteNetLib.Utils;
using Share.NetworkV2.TransportData.Define;

namespace Share.NetworkV2.TransportData.Header
{
    public class RequestHeader : INetDataHeader
    {
        public RequestHeader()
        {
        }

        public RequestHeader(ENetType netType, int netCommand, string token = null) : this()
        {
            NetType = netType;
            NetCommand = netCommand;
            Token = token;
        }

        public string Token { get; set; }

        public int RequestId { get; set; }
        public ENetType NetType { get; set; }
        public int NetCommand { get; set; }

        public void Deserialize(NetDataReader reader)
        {
            NetType = (ENetType) reader.GetInt();
            NetCommand = reader.GetInt();
            RequestId = reader.GetInt();
            Token = reader.GetString();
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put((int) NetType);
            writer.Put((int) NetCommand);
            writer.Put(RequestId);
            writer.Put(Token);
        }
    }
}
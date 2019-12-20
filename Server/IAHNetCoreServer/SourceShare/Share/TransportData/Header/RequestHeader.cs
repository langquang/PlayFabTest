using System.Threading;
using LiteNetLib.Utils;
using SourceShare.Share.TransportData.Define;

namespace SourceShare.Share.TransportData.Header
{
    public class RequestHeader : INetDataHeader
    {
        public RequestHeader()
        {
            RequestId = GenID();
        }

        public RequestHeader(ENetType netType, ENetCommand netCommand, string token = null) : this()
        {
            NetType = netType;
            NetCommand = netCommand;
            Token = token;
        }

        public string Token { get; set; }

        public int RequestId { get; set; }
        public ENetType NetType { get; set; }
        public ENetCommand NetCommand { get; set; }

        public void Deserialize(NetDataReader reader)
        {
            NetType = (ENetType) reader.GetInt();
            NetCommand = (ENetCommand) reader.GetInt();
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

        #region STATIC FUNCTION: auto gen id

        private static volatile int IDMaker;

        private static int GenID()
        {
            var id = Interlocked.Increment(ref IDMaker);
            if (id == int.MaxValue)
                Interlocked.Exchange(ref IDMaker, 0);
            return id;
        }

        #endregion
    }
}
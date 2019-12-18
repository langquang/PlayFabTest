using LiteNetLib;
using LiteNetLib.Utils;

namespace IAHNetCoreServer.Server
{
    public class NetPlayer
    {
        private NetPeer       _connection;
        private NetDataWriter _writer;
        public string Token { get; private set; }

        public NetPlayer(NetPeer peer, string token)
        {
            _connection = peer;
            Token = token;
            if (IsValidConnection)
                _writer = new NetDataWriter();
        }

        public bool IsValidConnection => _connection != null && _connection.ConnectionState == ConnectionState.Connected && string.IsNullOrEmpty(Token);

        public virtual void Response(Response netData)
        {
            // clear already data in writer
            _writer.Reset();
            // write data 
            netData.Serialize(_writer);
            // send
            _connection.Send(_writer, DeliveryMethod.ReliableOrdered);
            _connection.Flush(); // mask immediate send
        }
    }
}
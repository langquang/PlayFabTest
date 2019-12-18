using LiteNetLib;
using LiteNetLib.Utils;

namespace IAHNetCoreServer.Server
{
    public class NetPlayer
    {
        private NetPeer       _connection;
        private NetDataWriter _writer;
        private string        _token;

        public NetPeer Connection => _connection;

        public void InitNetwork(NetPeer peer, string token)
        {
            _connection = peer;
            _token = token;
            if (IsValidConnection)
                _writer = new NetDataWriter();
        }

        public bool IsValidConnection => _connection != null && _connection.ConnectionState == ConnectionState.Connected && string.IsNullOrEmpty(_token);

        public virtual void Send(INetData netData)
        {
            _writer.Reset();
            _writer.Put((int) netData.GetNetType());
            _writer.Put((int) netData.GetNetCommand());
            _writer.Put(_token);
            netData.Write(_writer);
            _connection.Send(_writer, DeliveryMethod.ReliableOrdered);
            _connection.Flush();
        }
    }
}
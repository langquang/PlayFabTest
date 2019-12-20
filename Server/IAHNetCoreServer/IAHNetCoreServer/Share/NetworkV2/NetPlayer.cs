using IAHNetCoreServer.Share.Router;
using IAHNetCoreServer.Share.TransportData.Base;
using IAHNetCoreServer.Share.TransportData.Header;
using LiteNetLib;
using LiteNetLib.Utils;

namespace IAHNetCoreServer.Share.NetworkV2
{
    public class NetPlayer
    {
        private readonly NetPeer         _connection;
        private readonly NetDataWriter   _writer;
        private          ClientNetRouter _router;

        public string Token { get; }
        public RequestHeader RequestHeader { get; }

        public bool IsValidConnection => _connection != null && _connection.ConnectionState == ConnectionState.Connected && string.IsNullOrEmpty(Token);

        public NetPlayer(NetPeer peer, string token = null, ClientNetRouter router = null)
        {
            _connection = peer;
            Token = token;
            _router = router;
            
            if (IsValidConnection)
            {
                _writer = new NetDataWriter();
                RequestHeader = new RequestHeader();
            }
        }

        public virtual void Send(INetData netData)
        {
            // clear already data in writer
            _writer.Reset();
            // write data 
            _router.WritePack(netData, _writer);
            // send
            _connection.Send(_writer, DeliveryMethod.ReliableOrdered);
            _connection.Flush(); // mask immediate send
        }
    }
}
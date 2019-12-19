using IAHNetCoreServer.Share.TransportData.Base;
using IAHNetCoreServer.Share.TransportData.Header;
using LiteNetLib;
using LiteNetLib.Utils;

namespace IAHNetCoreServer.Share.NetworkV2
{
    public class NetPlayer
    {
        private readonly NetPeer       _connection;
        private readonly NetDataWriter _writer;

        public string Token { get; }
        public RequestHeader RequestHeader { get; }

        public bool IsValidConnection => _connection != null && _connection.ConnectionState == ConnectionState.Connected && string.IsNullOrEmpty(Token);

        public NetPlayer(NetPeer peer, string token = null)
        {
            _connection = peer;
            Token = token;
            if (IsValidConnection)
            {
                _writer = new NetDataWriter();
                RequestHeader = new RequestHeader();
            }
        }
        
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

        public virtual void Request(Request request)
        {
            // clear already data in writer
            _writer.Reset();
            // write data 
            request.Serialize(_writer);
            // send
            _connection.Send(_writer, DeliveryMethod.ReliableOrdered);
            _connection.Flush(); // mask immediate send
        }
    }
}
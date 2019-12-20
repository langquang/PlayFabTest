using System;
using LiteNetLib;
using LiteNetLib.Utils;
using SourceShare.Share.Router;
using SourceShare.Share.TransportData.Base;
using SourceShare.Share.TransportData.Header;

namespace IAHNetCoreServer.Share.NetworkV2
{
    public class NetPlayer
    {
        private readonly NetPeer                   _connection;
        private readonly bool                      _isClient;
        private readonly NetRouter<ResponseHeader> _router;
        private readonly NetDataWriter             _writer;

        public NetPlayer(NetPeer peer, NetRouter<ResponseHeader> router, bool isClient, string token)
        {
            _connection = peer;
            _router = router;
            _isClient = isClient;
            Token = token;

            if (!_isClient && IsValidConnection)
            {
                _writer = new NetDataWriter();
                RequestHeader = new RequestHeader();
            }
        }

        public string Token { get; }
        public RequestHeader RequestHeader { get; }

        public bool IsValidConnection => _connection != null && _connection.ConnectionState == ConnectionState.Connected && !string.IsNullOrEmpty(Token);

        public void Send(INetData netData)
        {
            // Secret key between client and server
            if (Token != null && netData.Header is RequestHeader header)
                header.Token = Token;

            // clear already data in writer
            _writer.Reset();
            // write data 
            netData.Serialize(_writer);
            // send
            _connection.Send(_writer, DeliveryMethod.ReliableOrdered);
            _connection.Flush(); // mask immediate send
        }

        public void SendRequest<TNetResponse>(INetData request, Action<TNetResponse, NetPlayer> onSuccess, Action<int> onError, Action<TNetResponse, NetPlayer> onFinally) where TNetResponse : INetData
        {
            if (onSuccess != null || onError != null || onFinally != null)
                _router.SubscribeWaitingRequest(request, onSuccess, onError, onFinally);
            Send(request);
        }
    }
}
using System;
using LiteNetLib;
using LiteNetLib.Utils;
using SourceShare.Share.NetworkV2.Router;
using SourceShare.Share.NetworkV2.TransportData.Base;
using SourceShare.Share.NetworkV2.TransportData.Header;

namespace SourceShare.Share.NetworkV2
{
    public class NetPlayer
    {
        private readonly NetPeer       _connection;
        private readonly bool          _isClient;
        private readonly NetRouter     _router;
        private          NetDataWriter _writer;
        private          string        _token;

        public bool IsLogined { get; set; }
        public string PlayerId { get; private set; }

        public NetPlayer(string playerId, NetPeer peer, NetRouter router, bool isClient, string token)
        {
            PlayerId = playerId;
            _connection = peer;
            _router = router;
            _isClient = isClient;
            Token = token;
        }

        public string Token
        {
            get => _token;
            set
            {
                _token = value;
                if (_writer == null)
                {
                    _writer = new NetDataWriter();
                }
            }
        }

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

        public void SendRequest<TNetResponse>(INetData request, Action<TNetResponse, NetPlayer> onSuccess, Action<int> onError) where TNetResponse : INetData
        {
            SendRequest<TNetResponse>(request, onSuccess, onError, null, 0);
        }

        public void SendRequest<TNetResponse>(INetData request, Action<TNetResponse, NetPlayer> onSuccess, Action<int> onError, Action<TNetResponse, NetPlayer> onFinally) where TNetResponse : INetData
        {
            SendRequest<TNetResponse>(request, onSuccess, onError, onFinally, 0);
        }

        public void SendRequest<TNetResponse>(INetData request, Action<TNetResponse, NetPlayer> onSuccess, Action<int> onError, Action<TNetResponse, NetPlayer> onFinally, int timeOut) where TNetResponse : INetData
        {
            if (onSuccess != null || onError != null || onFinally != null)
                _router.SubscribeWaitingRequest(request, onSuccess, onError, onFinally, timeOut);
            Send(request);
        }
    }
}
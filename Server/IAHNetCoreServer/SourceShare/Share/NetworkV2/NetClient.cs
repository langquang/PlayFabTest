using System;
using System.Net;
using System.Net.Sockets;
using LiteNetLib;
using SourceShare.Share.NetworkV2.Client;

namespace SourceShare.Share.NetworkV2
{
    public class NetClient
    {
        private readonly ClientReceiveNetDataHandler _handler;

        // Read only props
        private readonly string     _name;
        private          NetManager _client;
        private          NetPeer    _connection;

        public Action<NetClient, NetManager> CustomParamsEvent;

        public NetClient(string name, ClientReceiveNetDataHandler handler)
        {
            _name = name;
            _handler = handler;
        }

        public EventBasedNetListener Listener { get; set; }

        public void Init()
        {
            Listener = new EventBasedNetListener();
            _client = new NetManager(Listener);

            Listener.PeerConnectedEvent += OnPeerConnected;
            Listener.PeerDisconnectedEvent += OnPeerDisconnected;
            Listener.NetworkReceiveEvent += OnNetworkReceive;
            Listener.NetworkReceiveUnconnectedEvent += OnNetworkReceiveUnconnected;
            Listener.NetworkLatencyUpdateEvent += OnNetworkLatencyUpdate;
            Listener.ConnectionRequestEvent += OnConnectionRequest;
            Listener.NetworkErrorEvent += OnNetworkError;

            if (CustomParamsEvent != null)
                //Console.WriteLine("Start client with custom params");
                CustomParamsEvent.Invoke(this, _client);
            else
                //Console.WriteLine("Start client with default params");
                SetDefaultParams();
        }

        public void SetDefaultParams()
        {
            _client.UpdateTime = 15;
            _client.DisconnectTimeout = 10000;
            _client.PingInterval = 1000;
            _client.ReuseAddress = true;
            _client.ReconnectDelay = 200;
            _client.UnconnectedMessagesEnabled = true;
        }

        public void Start(string address, int port, string key)
        {
            if (_connection != null) throw new Exception("NetClient already start!");

            _client.Start();
            _connection = _client.Connect(address, port, key);
        }

        public void Stop()
        {
            _client.Stop();
        }

        public void Update()
        {
            if (_connection != null) _client.PollEvents();
        }

        private void OnPeerConnected(NetPeer peer)
        {
        }

        private void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
        {
        }

        private void OnNetworkError(IPEndPoint endPoint, SocketError socketErrorCode)
        {
        }

        private void OnNetworkReceive(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod)
        {
            _handler.Perform(this, peer, reader, deliveryMethod);
            // reader.Recycle();
        }

        private void OnNetworkReceiveUnconnected(IPEndPoint remoteEndPoint, NetPacketReader reader, UnconnectedMessageType messageType)
        {
        }

        private void OnNetworkLatencyUpdate(NetPeer peer, int latency)
        {
        }

        private void OnConnectionRequest(ConnectionRequest request)
        {
        }
    }
}
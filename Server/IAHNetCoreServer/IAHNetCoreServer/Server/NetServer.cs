using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using LiteNetLib;

namespace IAHNetCoreServer.Server
{
    public class NetServer
    {
        private string                 _name;
        private IReceiveNetDataHandler _handler;
        private EventBasedNetListener  _listener;
        private NetManager             _server;

        private SimplePoolWriter _poolWriter;
        private bool             _running;
        private string           _acceptKey;

        public Action<NetServer, NetManager> CustomParamsEvent;


        public NetServer(string name, IReceiveNetDataHandler handler)
        {
            _name = name;
            _handler = handler;
        }

        public void Init()
        {
            _listener = new EventBasedNetListener();
            _server = new NetManager(_listener);
            _poolWriter = new SimplePoolWriter();

            _listener.PeerConnectedEvent += OnPeerConnected;
            _listener.PeerDisconnectedEvent += OnPeerDisconnected;
            _listener.NetworkReceiveEvent += OnNetworkReceive;
            _listener.NetworkReceiveUnconnectedEvent += OnNetworkReceiveUnconnected;
            _listener.NetworkLatencyUpdateEvent += OnNetworkLatencyUpdate;
            _listener.ConnectionRequestEvent += OnConnectionRequest;
            _listener.NetworkErrorEvent += OnNetworkError;

            if (CustomParamsEvent != null)
            {
                Console.WriteLine($"Start server {_name} with custom params");
                CustomParamsEvent.Invoke(this, _server);
            }
            else
            {
                Console.WriteLine($"Start server {_name} with default params");
                SetDefaultParams();
            }
        }

        public void SetDefaultParams()
        {
            _server.DiscoveryEnabled = false;
            _server.UnconnectedMessagesEnabled = true;
            _server.UpdateTime = 15;
            _server.DisconnectTimeout = 10000;
            _server.PingInterval = 1000;
            _server.ReuseAddress = true;
        }

        public void Start(int port, string acceptKey)
        {
            if (_server != null)
            {
                throw new Exception("NetServer already start!");
            }

            Init();
            _acceptKey = acceptKey;
            _server.Start(port);
            _running = true;
            Task.Factory.StartNew(async () =>
            {
                while (_running)
                {
                    _server.PollEvents();
                    await Task.Delay(1000 / 60);
                }

                _server.DisconnectAll();
                _server.Stop();
            }, TaskCreationOptions.LongRunning);
        }

        public void Stop()
        {
            _running = false;
        }

        private void OnPeerConnected(NetPeer peer)
        {
            Console.WriteLine($"[Server] Peer connected: {peer.EndPoint}, thread={ThreadHelper.GetCurrentThreadName("Socket")}");
            var peers = _server.GetPeers(ConnectionState.Connected);
            foreach (var netPeer in peers)
            {
                Console.WriteLine("ConnectedPeersList: id={0}, ep={1}", netPeer.Id, netPeer.EndPoint);
            }
        }

        private void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
        {
            Console.WriteLine("[Server] Peer disconnected: " + peer.EndPoint + ", reason: " + disconnectInfo.Reason);
        }

        private void OnNetworkError(IPEndPoint endPoint, SocketError socketErrorCode)
        {
            Console.WriteLine("[Server] error: " + socketErrorCode);
        }

        private void OnNetworkReceive(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod)
        {
            _handler.Login(this, peer, reader, deliveryMethod);
        }

        private void OnNetworkReceiveUnconnected(IPEndPoint remoteEndPoint, NetPacketReader reader, UnconnectedMessageType messageType)
        {
            Console.WriteLine("[Server] ReceiveUnconnected {0}. From: {1}. Data: {2}", messageType, remoteEndPoint, reader.GetString(100));
            var writer = _poolWriter.Spawn();
            writer.Put("SERVER DISCOVERY RESPONSE");
            _server.SendUnconnectedMessage(writer, remoteEndPoint);
            _poolWriter.UnSpawn(writer);
        }

        private void OnNetworkLatencyUpdate(NetPeer peer, int latency)
        {
        }

        private void OnConnectionRequest(ConnectionRequest request)
        {
            request.AcceptIfKey(_acceptKey);
        }
    }
}
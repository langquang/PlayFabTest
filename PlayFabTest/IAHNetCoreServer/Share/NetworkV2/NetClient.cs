using System;
using LiteNetLib;
using LiteNetLib.Utils;

namespace IAHNetCoreServer.Share.NetworkV2
{
    public class NetClient
    {
        private NetManager            _client;
        private NetPeer               _connection;
        private EventBasedNetListener _listener;
        private NetDataWriter         _writer;

        public Action<NetClient, NetManager> CustomParamsEvent;

        public void Start(string address, int port, string key)
        {
            _listener = new EventBasedNetListener();
            _client = new NetManager(_listener);

            if (CustomParamsEvent != null)
            {
                Console.WriteLine("Start client with custom params");
                CustomParamsEvent.Invoke(this, _client);
            }
            else
            {
                Console.WriteLine("Start client with default params");
                SetDefaultParams();
            }

            _client.Start();
            _connection = _client.Connect(address, port, key);

            _listener.NetworkReceiveEvent += (fromPeer, dataReader, deliveryMethod) =>
            {
//                Console.WriteLine("We got: {0}", dataReader.GetString(100));
                dataReader.Recycle();
            };
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

        public void Stop()
        {
            _client.Stop();
        }

        public void Update()
        {
            if (_connection != null) _client.PollEvents();
        }
    }
}
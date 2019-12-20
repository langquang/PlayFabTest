using IAHNetCoreServer.Share.NetworkV2;
using LiteNetLib;
using SourceShare.Share.NetworkV2;
using SourceShare.Share.NetworkV2.Client;
using SourceShare.Share.Router;
using SourceShare.Share.TransportData;
using SourceShare.Share.TransportData.Define;
using SourceShare.Share.TransportData.Header;
using SourceShare.Share.TransportData.Misc;
using UnityClientLib.NetworkV2;
using UnityEngine;

namespace UnityClientLib.Logic.Client.ResponseHandler
{
    public class APINetworkHandler : ClientReceiveNetDataHandler
    {
        #region SINGLETON

        private static APINetworkHandler _instance;

        public static APINetworkHandler Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new APINetworkHandler();
                }

                return _instance;
            }
        }

        #endregion

        private readonly NetRouter<ResponseHeader> _router;

        private NetClient _netClient;
        private NetPlayer _player;

        public NetPlayer Player => _player;

        private string _curPlayFabId, _sessionTicket;

        public APINetworkHandler()
        {
            // ===================== Make a network ====================================================================
            _netClient = new NetClient("api-server", this);
            _netClient.CustomParamsEvent += (client, manager) => { client.SetDefaultParams(); }; // can custom network option here
            _netClient.Init();                                                                   // init network from custom network option
            _netClient.Listener.PeerConnectedEvent += OnConnected;                               // start login after connected

            // ===================== Config Router =====================================================================
            _router = new NetRouter<ResponseHeader>(new TimeOutChecker(this));
            // Register headers
            _router.RegisterHeader<RequestHeader>(() => new RequestHeader());
            _router.RegisterHeader<ResponseHeader>(() => new ResponseHeader());
            // Subscribe income Handler (one way)
            _router.Subscribe<TestRequest>(ENetCommand.TEST_REQUEST, OnReceiveTestRequest);
            _router.Subscribe<TestResponse>(ENetCommand.TEST_REQUEST, OnReceiveTestResponse);
        }

        public void StartConnect(string curPlayFabId, string sessionTicket)
        {
            _curPlayFabId = curPlayFabId;
            _sessionTicket = sessionTicket;
            _netClient.Start("127.0.0.1", 8000, "ButinABC");
        }

        private void OnConnected(NetPeer peer)
        {
            // Debug.Log("Try to Login to server");
            var loginRequest = new LoginRequest(_curPlayFabId, _sessionTicket);
            _player = new NetPlayer(peer, _router, true, null);
            _player.Send(loginRequest);
        }

        public void Update()
        {
            _netClient?.Update();
        }

        public override void Perform(NetClient netClient, NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod)
        {
            _router.ReadAllPackets(reader, _player);
        }

        public override void OnTimeRequestTimeOut(RequestTimeOut requestTimeOut)
        {
            Debug.Log($"OnTimeRequestTimeOut {requestTimeOut.command}");
        }

        public void OnReceiveTestRequest(TestRequest request, NetPlayer player)
        {
        }

        public void OnReceiveTestResponse(TestResponse response, NetPlayer player)
        {
        }

        public void OnSendTestSuccess(TestRequest request, TestResponse response, NetPlayer player)
        {
        }

        public void OnSendTestError(int error)
        {
        }

        public void OnSendTestFinally(TestRequest request, TestResponse response, NetPlayer player)
        {
        }
    }
}
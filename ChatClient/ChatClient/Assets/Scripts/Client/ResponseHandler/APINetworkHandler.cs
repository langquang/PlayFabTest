using System;
using LiteNetLib;
using PlayFabCustom;
using PlayFabShare;
using SourceShare.Share.NetRequest;
using SourceShare.Share.NetRequest.Config;
using SourceShare.Share.NetworkV2;
using SourceShare.Share.NetworkV2.Client;
using SourceShare.Share.NetworkV2.Router;
using SourceShare.Share.NetworkV2.TransportData.Base;
using SourceShare.Share.NetworkV2.TransportData.Header;
using SourceShare.Share.NetworkV2.TransportData.Misc;
using SourceShare.Share.NetworkV2.Utils;
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

        private readonly NetRouter<NetPlayer> _router;
        private readonly NetClient            _netClient;

        private NetPlayer _player;

        public NetPlayer Player => _player;

        private string       _curPlayFabId, _sessionTicket;
        private CreateParams _createParams;

        private APINetworkHandler()
        {
            // ===================== Make a network ====================================================================
            _netClient = new NetClient("api-server", this);
            _netClient.CustomParamsEvent += (client, manager) => { client.SetDefaultParams(); }; // can custom network option here
            _netClient.Init();                                                                   // init network from custom network option
            _netClient.Listener.PeerConnectedEvent += OnConnected;                               // start login after connected

            // ===================== Config Router =====================================================================
            _router = new NetRouter<NetPlayer>(new TimeOutChecker(this));
            // Register headers
            _router.RegisterHeader<RequestHeader>(() => new RequestHeader());
            _router.RegisterHeader<ResponseHeader>(() => new ResponseHeader());
            // Subscribe income Handler (one way)
            _router.Subscribe<TestRequest>(NetAPICommand.TEST_REQUEST, OnReceiveTestRequest);
            _router.Subscribe<TestResponse>(NetAPICommand.TEST_REQUEST, OnReceiveTestResponse);
        }

        public void StartConnect(string curPlayFabId, string sessionTicket, CreateParams createParams)
        {
            _curPlayFabId = curPlayFabId;
            _sessionTicket = sessionTicket;
            _createParams = createParams;
            // _netClient.Start("127.0.0.1", 8000, "ButinABC");
            _netClient.Start("35.187.249.32", 8000, "ButinABC");
        }

        private void OnConnected(NetPeer peer)
        {
            Debugger.Write("API Server: OnConnected");
            var loginRequest = new LoginRequest(_curPlayFabId, _sessionTicket);
            _player = new NetPlayer(_curPlayFabId, peer, null);
            _router.SendRequest<LoginResponse>(_player, loginRequest, (response, player) =>
            {
                Debugger.Write($"Login successful, token = {response.token}");
                player.Token = response.token;

                if (_createParams != null)
                {
                    if (_createParams.needRegisterMasterAccount)
                        CreateMasterAccountHandler(_createParams.server);
                    else if (_createParams.needRegisterNodeAccount)
                        CreateNodeAccountHandler(_createParams.server, _createParams.masterId);
                }
            }, i => Debugger.Write($"Login error = {i}"));
        }

        public void Update()
        {
            _netClient?.Update();
        }

        public void Stop()
        {
            _netClient?.Stop();
        }

        public override void Perform(NetClient netClient, NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod)
        {
            _router.ReadAllPackets(reader, _player);
        }

        public override void OnTimeRequestTimeOut(RequestTimeOut requestTimeOut)
        {
            Debug.Log($"OnTimeRequestTimeOut {requestTimeOut.command}");
        }

        public void SendRequest<TNetResponse>(INetData request, Action<TNetResponse, NetPlayer> onSuccess, Action<int> onError) where TNetResponse : INetData
        {
            _router.SendRequest(_player, request, onSuccess, onError, null, 0);
        }

        public void SendRequest<TNetResponse>(INetData request, Action<TNetResponse, NetPlayer> onSuccess, Action<int> onError, Action<TNetResponse, NetPlayer> onFinally, int timeOut) where TNetResponse : INetData
        {
            _router.SendRequest(_player, request, onSuccess, onError, onFinally, timeOut);
        }

        public void OnReceiveTestRequest(TestRequest request, NetPlayer player)
        {
        }

        public void OnReceiveTestResponse(TestResponse response, NetPlayer player)
        {
        }

        private static void CreateMasterAccountHandler(int server)
        {
            Debugger.Write("Try to CreateMasterAccountRequest");
            var request = new CreateMasterAccountRequest(server);
            APINetworkHandler.Instance.SendRequest<CreateMasterAccountResponse>(request, (response, player) => { Debugger.Write("Create node account successful"); }, i => { Debugger.Write($"Create Master Account fail, code = {i};"); });
        }

        private static void CreateNodeAccountHandler(int server, string masterId)
        {
            Debugger.Write("Try to CreateNodeAccountRequest");
            var request = new CreateNodeAccountRequest(server, masterId);
            APINetworkHandler.Instance.SendRequest<CreateNodeAccountResponse>(request, (response, player) => { Debugger.Write("Create node account successful"); }, i => { Debugger.Write($"Create Master Account fail, code = {i}"); });
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
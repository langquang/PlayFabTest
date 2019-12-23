using System;
using System.IO;
using System.Threading.Tasks;
using IAHNetCoreServer.Server;
using IAHNetCoreServer.Share.NetworkV2;
using LiteNetLib;
using MessagePack;
using PlayFab;
using PlayFab.ServerModels;
using SourceShare.Share.Router;
using SourceShare.Share.TransportData;
using SourceShare.Share.TransportData.Base;
using SourceShare.Share.TransportData.define;
using SourceShare.Share.TransportData.Define;
using SourceShare.Share.TransportData.Header;
using SourceShare.Share.TransportData.Misc;
using SourceShare.Share.Utils;

namespace IAHNetCoreServer.Logic.Server.RequestHandlers
{
    public class EntryHandler : ServerReceiveNetDataHandler<NetPlayer>
    {
        private readonly NetRouter _router;

        public EntryHandler()
        {
            _router = new NetRouter(new TimeOutChecker(this));
            // Register headers
            _router.RegisterHeader<RequestHeader>(() => new RequestHeader());
            _router.RegisterHeader<ResponseHeader>(() => new ResponseHeader());

            _router.Subscribe<TestRequest>(NetAPICommand.TEST_REQUEST, OnTestHandler);
        }


        /// <summary>
        /// Gen a session ticket
        /// </summary>
        /// <returns></returns>
        private static string GenToken()
        {
            return Path.GetRandomFileName();
        }

        public override (NetPlayer, INetData) BeginLogin(NetPeer peer, NetPacketReader reader)
        {
            var header = _router.ReadHeader(reader);
            if (header.NetType != ENetType.REQUEST || header.NetCommand != NetAPICommand.LOGIN_REQUEST)
            {
                Debugger.Write("invalid login header");
                peer.Disconnect();
                return (null, null);
            }

            var loginRequest = MessagePackSerializer.Deserialize<LoginRequest>(reader.GetBytesWithLength());
            loginRequest.Header = header;
            var player = new NetPlayer(loginRequest.playerId, peer, _router, false, GenToken());
            if (!loginRequest.IsValid())
            {
                Debugger.Write("invalid login request");
                ResponseError(player, loginRequest, 1);
                return (null, null);
            }

            var curOnlinePlayer = OnlinePlayers.Instance.Players.FindPlayer(loginRequest.playerId);
            if (curOnlinePlayer != null)
            {
                Debugger.Write($"OnReady online");
                ResponseError(player, loginRequest, 2);
                return (null, null);
            }

            // todo: check parallel login
            return (player, loginRequest);
        }

        public override async Task<NetPlayer> VerifyLogin(NetPlayer player, INetData request)
        {
            PlayFabSettings.staticSettings.TitleId = "20443";
            PlayFabSettings.staticSettings.DeveloperSecretKey = "U7XWD3YGJFIOD3HX7F74J75RYOOGE4UHO75KGMK7APBBQUPBUJ";

            var loginRequest = (LoginRequest) request;
            var result = await PlayFabServerAPI.AuthenticateSessionTicketAsync(new AuthenticateSessionTicketRequest() {SessionTicket = loginRequest.sessionTicket}); // asynchronous here, make a sub task, calling thread is free and comeback to next line of code when sub task is done
            if (result.Error == null)
            {
                Debugger.Write($"UserName: {result.Result.UserInfo.Username}");
            }
            else
            {
                Debugger.Write("Invalid Session Ticket");
            }

            player.IsLogined = true;
            OnlinePlayers.Instance.Players.AddPlayer(loginRequest.playerId, player);
            // response to client
            var response = new LoginResponse(new ResponseHeader(loginRequest.Header)) {token = player.Token};
            player.Send(response);
            return player;
        }

        public override void Perform(NetPlayer player, NetPacketReader reader)
        {
            try
            {
                _router.ReadAllPackets(reader, player);
            }
            catch (Exception e)
            {
                Debugger.Write(e.ToString());
                throw;
            }
        }

        public override void OnDisconnect(NetPlayer player)
        {
            OnlinePlayers.Instance.Players.RemovePlayer(player.PlayerId);
        }

        public override void OnTimeRequestTimeOut(RequestTimeOut requestTimeOut)
        {
            Debugger.Write($"RequestTimeOut {requestTimeOut.command}");
        }

        public static INetData ResponseError(NetPlayer player, INetData request, int errorCode)
        {
            var response = new CommonErrorResponse(request, errorCode);
            player.Send(response);
            return response;
        }

        private static void OnTestHandler(TestRequest request, NetPlayer player)
        {
            Debugger.Write($"Server receive a Test Command with content={request.msg}");
            var response = new TestResponse(request) {msg = "A response of test command from server"};
            player.Send(response);
        }
    }
}
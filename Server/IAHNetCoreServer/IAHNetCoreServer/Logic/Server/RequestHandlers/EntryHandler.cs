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
using SourceShare.Share.TransportData.Define;
using SourceShare.Share.TransportData.Header;
using SourceShare.Share.TransportData.Misc;

namespace IAHNetCoreServer.Logic.Server.RequestHandlers
{
    public class EntryHandler : ServerReceiveNetDataHandler
    {
        private readonly GroupPlayers<NetPeer>     _groupPlayers; // store active Players, peer id -> Player 
        private readonly NetRouter<ResponseHeader> _router;

        public EntryHandler()
        {
            _groupPlayers = new GroupPlayers<NetPeer>();
            _router = new NetRouter<ResponseHeader>(new TimeOutChecker(this));
            // Register headers
            _router.RegisterHeader<RequestHeader>(() => new RequestHeader());
            _router.RegisterHeader<ResponseHeader>(() => new ResponseHeader());

            _router.Subscribe<TestRequest>(ENetCommand.TEST_REQUEST, OnTestHandler);
        }


        /// <summary>
        /// Gen a session ticket
        /// </summary>
        /// <returns></returns>
        private static string GenToken()
        {
            return Path.GetRandomFileName();
        }

        /// <summary>
        ///  Handler Login Request
        /// </summary>
        /// <param name="netServer"></param>
        /// <param name="peer"></param>
        /// <param name="reader"></param>
        /// <param name="deliveryMethod"></param>
        public override Task<INetData> Perform(NetServer netServer, NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod)
        {
            Console.WriteLine($"Receive from peer: {peer.Id}");
            var player = peer.Tag;
            if (player == null)
            {
                return BeginLogin(netServer, peer, reader, deliveryMethod);
            }
            else
            {
                RequestAfterLogin(netServer, (NetPlayer) player, reader, deliveryMethod);
                return null;
            }
        }

        /// <summary>
        ///  Handler Login Request
        /// </summary>
        /// <param name="netServer"></param>
        /// <param name="peer"></param>
        /// <param name="reader"></param>
        /// <param name="deliveryMethod"></param>
        public override async Task<INetData> BeginLogin(NetServer netServer, NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod)
        {
            var header = _router.ReadHeader(reader);
            if (header.NetType != ENetType.REQUEST || header.NetCommand != ENetCommand.LOGIN_REQUEST)
            {
                Console.WriteLine($"invalid login header");
                peer.Disconnect();
                return null;
            }

            var loginRequest = MessagePackSerializer.Deserialize<LoginRequest>(reader.GetBytesWithLength());
            var player = new NetPlayer(peer, _router, false, GenToken());
            if (!loginRequest.IsValid())
            {
                Console.WriteLine($"invalid login request");
                return ResponseError(player, loginRequest, 1);
            }

            var curOnlinePlayer = OnlinePlayers.Instance.Players.FindPlayer(loginRequest.playerId);
            if (curOnlinePlayer != null)
            {
                Console.WriteLine($"OnReady online");
                return ResponseError(player, loginRequest, 2);
            }

            // todo: check parallel login

            // login success
            PlayFabSettings.staticSettings.TitleId = "20443";
            PlayFabSettings.staticSettings.DeveloperSecretKey = "U7XWD3YGJFIOD3HX7F74J75RYOOGE4UHO75KGMK7APBBQUPBUJ";
            var result = await PlayFabServerAPI.AuthenticateSessionTicketAsync(new AuthenticateSessionTicketRequest() {SessionTicket = loginRequest.sessionTicket}); // asynchronous here, make a sub task, calling thread is free and comeback to next line of code when sub task is done
            if (result.Error == null)
            {
                Console.WriteLine($"UserName: {result.Result.UserInfo.Username}");
            }
            else
            {
                Console.WriteLine("Invalid Session Ticket");
            }

            peer.Tag = player;
            OnlinePlayers.Instance.Players.AddPlayer(loginRequest.playerId, player);
            // response to client
            var response = new LoginResponse(new ResponseHeader(header)) {playerId = "PlayerId from server", sessionTicket = "Ticket from server"};
            player.Send(response);
            return response;
        }

        public override void RequestAfterLogin(NetServer netServer, NetPlayer player, NetPacketReader reader, DeliveryMethod deliveryMethod)
        {
            _router.ReadAllPackets(reader, player);
        }

        public override void OnTimeRequestTimeOut(RequestTimeOut requestTimeOut)
        {
            Console.WriteLine($"RequestTimeOut {requestTimeOut.command}");
        }

        public static INetData ResponseError(NetPlayer player, INetData request, int errorCode)
        {
            var response = new CommonErrorResponse(request, errorCode);
            player.Send(response);
            return response;
        }

        private static void OnTestHandler(TestRequest request, NetPlayer player)
        {
            Console.WriteLine($"Server receive a Test Command with content={request.msg}");
            var response = new TestResponse(request) {msg = "A response of test command from server"};
            player.Send(response);
        }
    }
}
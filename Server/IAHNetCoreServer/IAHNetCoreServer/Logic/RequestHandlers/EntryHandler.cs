using System;
using System.IO;
using System.Threading.Tasks;
using IAHNetCoreServer.Server;
using IAHNetCoreServer.Share.NetworkV2;
using IAHNetCoreServer.Share.Router;
using IAHNetCoreServer.Share.TransportData;
using IAHNetCoreServer.Share.TransportData.Base;
using IAHNetCoreServer.Share.TransportData.Define;
using IAHNetCoreServer.Share.TransportData.Header;
using LiteNetLib;
using MessagePack;
using PlayFab;
using PlayFab.ServerModels;

namespace IAHNetCoreServer.Logic.RequestHandlers
{
    public class EntryHandler : IReceiveNetDataHandler
    {
        private readonly GroupPlayers<int>        _groupPlayers; // store active Players, peer id -> Player 
        private readonly NetRouter<RequestHeader> _netRouter;

        public EntryHandler()
        {
            _groupPlayers = new GroupPlayers<int>();
            _netRouter = new NetRouter<RequestHeader>();
            _netRouter.Subscribe<TestRequest>(ENetCommand.TEST_REQUEST, OnTestHandler);
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
        public Task<Response> Perform(NetServer netServer, NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod)
        {
            var player = _groupPlayers.FindPlayer(peer.Id);
            if (player == null)
            {
                return BeginLogin(netServer, peer, reader, deliveryMethod);
            }
            else
            {
                RequestAfterLogin(netServer, player, reader, deliveryMethod);
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
        public async Task<Response> BeginLogin(NetServer netServer, NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod)
        {
            var header = new RequestHeader();
            header.Deserialize(reader);
            if (header.NetType != ENetType.REQUEST || header.NetCommand != ENetCommand.LOGIN_REQUEST)
            {
                peer.Disconnect();
                return null;
            }

            var loginRequest = MessagePackSerializer.Deserialize<LoginRequest>(reader.GetBytesWithLength());
            var player = new NetPlayer(peer, GenToken());
            if (!loginRequest.IsValid())
            {
                return ResponseError(player, header, 1);
            }

            var curOnlinePlayer = OnlinePlayers.Instance.Players.FindPlayer(loginRequest.playerId);
            if (curOnlinePlayer != null)
            {
                return ResponseError(player, header, 2);
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

            _groupPlayers.AddPlayer(peer.Id, player);
            OnlinePlayers.Instance.Players.AddPlayer(loginRequest.playerId, player);
            // response to client
            var response = new LoginResponse(new ResponseHeader(header)) {playerId = "PlayerId from server", sessionTicket = "Ticket from server"};
            player.Response(response);
            return response;
        }

        public void RequestAfterLogin(NetServer netServer, NetPlayer player, NetPacketReader reader, DeliveryMethod deliveryMethod)
        {
            _netRouter.ReadAllPackets(reader, player);
        }

        public static Response ResponseError(NetPlayer player, RequestHeader requestHeader, int errorCode)
        {
            var response = new CommonErrorResponse(new ResponseHeader(requestHeader, errorCode));
            player.Response(response);
            return response;
        }

        private static void OnTestHandler(RequestHeader header, TestRequest request, NetPlayer player)
        {
            Console.WriteLine($"Server receive a Test Command with content={request.msg}");
            var response = new TestResponse(new ResponseHeader(request.Header)) {msg = "A response of test command from server"};
            player.Response(response);
        }
    }
}
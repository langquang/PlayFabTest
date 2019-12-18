using System.IO;
using IAHNetCoreServer.NetData;
using IAHNetCoreServer.NetData.Header;
using LiteNetLib;
using MessagePack;

namespace IAHNetCoreServer.Server
{
    public class ServerHandler : IReceiveNetDataHandler
    {
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
        public void Login(NetServer netServer, NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod)
        {
            var header = new RequestHeader();
            header.Deserialize(reader);
            if (header.NetType != ENetType.REQUEST || header.NetCommand != ENetCommand.LOGIN_REQUEST)
            {
                peer.Disconnect();
                return;
            }
            
            var loginRequest = MessagePackSerializer.Deserialize<LoginRequest>(reader.GetBytesWithLength());
            var player = new NetPlayer(peer, GenToken());
            if (!loginRequest.IsValid())
            {
                var loginResponse = new LoginResponse(new ResponseHeader(header, 1));
                player.Response(loginResponse);
                return;
            }
            
            // login success
        }

        public void Perform(NetServer netServer, NetPlayer player, NetPacketReader reader, DeliveryMethod deliveryMethod)
        {
            throw new System.NotImplementedException();
        }
    }
}
using System.Threading.Tasks;
using IAHNetCoreServer.Share.NetworkV2;
using IAHNetCoreServer.Share.TransportData.Base;
using LiteNetLib;

namespace IAHNetCoreServer.Server
{
    public interface IReceiveNetDataHandler
    {
        Task<Response> Perform(NetServer netServer, NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod);
        Task<Response> BeginLogin(NetServer netServer, NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod);
        void RequestAfterLogin(NetServer netServer, NetPlayer player, NetPacketReader reader, DeliveryMethod deliveryMethod);
    }
}
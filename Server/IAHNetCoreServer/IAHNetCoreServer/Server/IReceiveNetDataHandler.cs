using LiteNetLib;

namespace IAHNetCoreServer.Server
{
    public interface IReceiveNetDataHandler
    {
        void Login(NetServer netServer, NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod);
        void Perform(NetServer netServer, NetPlayer player, NetPacketReader reader, DeliveryMethod deliveryMethod);
    }
}
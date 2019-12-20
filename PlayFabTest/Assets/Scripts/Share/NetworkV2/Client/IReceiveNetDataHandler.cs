using IAHNetCoreServer.Share.NetworkV2;
using LiteNetLib;

namespace IAHNetCoreServer.NetworkV2.Client
{
    public interface IReceiveNetDataHandler
    {
        void Perform(NetClient netClient, NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod);
    }
}
using LiteNetLib;
using Share.NetworkV2.Base;
using Share.NetworkV2.TransportData.Misc;

namespace Share.NetworkV2.Client
{
    public abstract class ClientReceiveNetDataHandler : IReceiveNetDataHandler
    {
        public abstract void Perform(NetClient netClient, NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod);
        public abstract void OnTimeRequestTimeOut(RequestTimeOut requestTimeOut);
    }
}
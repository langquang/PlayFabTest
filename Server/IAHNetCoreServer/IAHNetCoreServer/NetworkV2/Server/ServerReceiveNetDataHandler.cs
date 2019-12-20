using System.Threading.Tasks;
using IAHNetCoreServer.Share.NetworkV2;
using LiteNetLib;
using SourceShare.Share.NetworkV2.Base;
using SourceShare.Share.TransportData.Base;
using SourceShare.Share.TransportData.Header;
using SourceShare.Share.TransportData.Misc;

namespace IAHNetCoreServer.Server
{
    public abstract class ServerReceiveNetDataHandler : IReceiveNetDataHandler
    {
        public abstract Task<INetData> Perform(NetServer netServer, NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod);
        public abstract Task<INetData> BeginLogin(NetServer netServer, NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod);
        public abstract void RequestAfterLogin(NetServer netServer, NetPlayer player, NetPacketReader reader, DeliveryMethod deliveryMethod);
        public abstract void OnTimeRequestTimeOut(RequestTimeOut requestTimeOut);
    }
}
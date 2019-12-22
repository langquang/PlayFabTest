using System.Threading.Tasks;
using IAHNetCoreServer.Share.NetworkV2;
using LiteNetLib;
using SourceShare.Share.NetworkV2.Base;
using SourceShare.Share.TransportData.Base;
using SourceShare.Share.TransportData.Misc;

namespace IAHNetCoreServer.Server
{
    public abstract class ServerReceiveNetDataHandler<TPlayer> : IReceiveNetDataHandler where TPlayer : NetPlayer
    {
        public abstract (TPlayer, INetData) BeginLogin(NetPeer peer, NetPacketReader reader);
        public abstract Task<TPlayer> VerifyLogin(TPlayer player, INetData request);
        public abstract void Perform(NetPlayer player, NetPacketReader reader);
        public abstract void OnTimeRequestTimeOut(RequestTimeOut requestTimeOut);
        public abstract void OnDisconnect(TPlayer player);
    }
}
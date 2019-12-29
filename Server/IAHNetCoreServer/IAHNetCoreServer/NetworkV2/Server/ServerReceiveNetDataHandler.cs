using System.Threading.Tasks;
using LiteNetLib;
using SourceShare.Share.NetworkV2;
using SourceShare.Share.NetworkV2.Base;
using SourceShare.Share.NetworkV2.TransportData.Base;
using SourceShare.Share.NetworkV2.TransportData.Misc;

namespace NetworkV2.Server
{
    public abstract class ServerReceiveNetDataHandler<TPlayer> : IReceiveNetDataHandler where TPlayer : NetPlayer
    {
        public abstract (TPlayer, INetData) BeginLogin(NetPeer peer, NetPacketReader reader);
        public abstract Task<TPlayer> VerifyLogin(TPlayer player, INetData request);
        public abstract Task Perform(TPlayer player, NetPacketReader reader);
        public abstract void OnTimeRequestTimeOut(RequestTimeOut requestTimeOut);
        public abstract void OnDisconnect(TPlayer player);
    }
}
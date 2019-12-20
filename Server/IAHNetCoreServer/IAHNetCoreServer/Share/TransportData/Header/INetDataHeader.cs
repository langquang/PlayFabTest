using IAHNetCoreServer.Share.TransportData.Define;
using LiteNetLib.Utils;

namespace IAHNetCoreServer.Share.TransportData.Header
{
    public interface INetDataHeader
    {
        /// <summary>
        ///     Request - Response or Message
        /// </summary>
        ENetType NetType { get; set; }

        /// <summary>
        ///     Login, Chat ...
        /// </summary>
        ENetCommand NetCommand { get; set; }
        
        /// <summary>
        ///   id of packet
        /// </summary>
        int RequestId { get; set; }

        void Deserialize(NetDataReader reader);
        void Serialize(NetDataWriter writer);
    }
}
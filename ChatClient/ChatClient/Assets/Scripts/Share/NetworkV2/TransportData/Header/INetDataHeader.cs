using LiteNetLib.Utils;
using Share.NetworkV2.TransportData.Define;

namespace Share.NetworkV2.TransportData.Header
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
        int NetCommand { get; set; }

        /// <summary>
        ///     id of packet
        /// </summary>
        int RequestId { get; set; }

        void Deserialize(NetDataReader reader);
        void Serialize(NetDataWriter writer);
    }
}
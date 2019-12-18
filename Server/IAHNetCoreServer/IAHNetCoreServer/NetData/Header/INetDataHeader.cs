using IAHNetCoreServer.Server;
using LiteNetLib.Utils;

namespace IAHNetCoreServer.NetData.Header
{
    public interface INetDataHeader
    {
        /// <summary>
        /// Request - Response or Message
        /// </summary>
        ENetType NetType { get; set; }

        /// <summary>
        ///  Login, Chat ...
        /// </summary>
        ENetCommand NetCommand { get; set; }

        void Deserialize(NetDataReader reader);
        void Serialize(NetDataWriter writer);
    }
}
using LiteNetLib.Utils;

namespace IAHNetCoreServer.Server
{
    public interface INetData
    {
        /// <summary>
        /// Request - Response or Message
        /// </summary>
        ENetType GetNetType();

        /// <summary>
        ///  Login, Chat ...
        /// </summary>
        ENetCommand GetNetCommand();

        void Write(NetDataWriter writer);
    }
}
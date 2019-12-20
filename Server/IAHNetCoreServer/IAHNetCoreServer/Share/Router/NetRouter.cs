using System;
using System.Collections.Generic;
using IAHNetCoreServer.Share.NetworkV2;
using IAHNetCoreServer.Share.TransportData.Base;
using IAHNetCoreServer.Share.TransportData.Define;
using IAHNetCoreServer.Share.TransportData.Header;
using LiteNetLib.Utils;
using MessagePack;

namespace IAHNetCoreServer.Share.Router
{
    public class NetRouter<TNetHeader> where TNetHeader : INetDataHeader, new()
    {
        private delegate void SubscribeDelegate(TNetHeader header, NetDataReader reader, NetPlayer player);

        private readonly Dictionary<ENetCommand, SubscribeDelegate> _callbacks = new Dictionary<ENetCommand, SubscribeDelegate>();

        private SubscribeDelegate GetCallback(ENetCommand netCommand)
        {
            if (!_callbacks.TryGetValue(netCommand, out var action))
            {
                throw new ParseException($"Network Error: Not implemented {netCommand} yet!");
            }

            return action;
        }

        public void ReadPacket(NetDataReader reader, NetPlayer player)
        {
            var header = new TNetHeader();
            header.Deserialize(reader);
            GetCallback(header.NetCommand).Invoke(header, reader, player);
        }

        /// <summary>
        /// Reads all available data from NetDataReader and calls OnReceive delegates
        /// </summary>
        /// <param name="reader">NetDataReader with packets data</param>
        /// <param name="player">Argument that passed to OnReceivedEvent</param>
        /// <exception cref="ParseException">Malformed packet</exception>
        public void ReadAllPackets(NetDataReader reader, NetPlayer player)
        {
            while (reader.AvailableBytes > 0)
            {
                ReadPacket(reader, player);
            }
        }

        /// <summary>
        /// Register and subscribe to packet receive event (with userData)
        /// </summary>
        /// <param name="command">command id</param>
        /// <param name="onReceive">event that will be called when packet deserialized with ReadPacket method</param>
        /// <exception cref="InvalidTypeException"><typeparamref name="TNetHeader"/>'s fields are not supported, or it has no fields</exception>
        public void Subscribe<TNetData>(ENetCommand command, Action<TNetData, NetPlayer> onReceive) where TNetData : INetData
        {
            _callbacks[command] = (header, reader, player) =>
            {
                var request = MessagePackSerializer.Deserialize<TNetData>(reader.GetBytesWithLength());
                request.Header = header;
                onReceive.Invoke(request, player);
            };
        }
    }
}
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
    public class NetRouter<TWaitingResponseHeader> where TWaitingResponseHeader : ResponseHeader
    {
        private delegate void SubscribeIncomeDelegate(INetDataHeader header, NetDataReader reader, NetPlayer player);

        private delegate void SubscribeWaitingResponseDelegate(TWaitingResponseHeader header, NetDataReader reader, NetPlayer player);

        // subscribe with ENetCommand
        private readonly Dictionary<ENetCommand, SubscribeIncomeDelegate> _incomeRequestCallbacks = new Dictionary<ENetCommand, SubscribeIncomeDelegate>();

        // subscribe with Request Id
        private readonly Dictionary<int, SubscribeWaitingResponseDelegate> _waitingForResponseCallbacks = new Dictionary<int, SubscribeWaitingResponseDelegate>();

        private readonly Dictionary<ulong, Func<INetDataHeader>> _headerConstructors = new Dictionary<ulong, Func<INetDataHeader>>();

        public Action<INetData> OnWaitingTimeOutEvent;

        public void RegisterHeader<T>(Func<T> headerConstructor) where T : INetDataHeader, new()
        {
            var t = typeof(T);
            var hash = HashName.GetHash(t);
            _headerConstructors[hash] = () => headerConstructor.Invoke();
        }

        private SubscribeIncomeDelegate GetIncomeRequestCallback(ENetCommand netCommand)
        {
            if (!_incomeRequestCallbacks.TryGetValue(netCommand, out var action))
            {
                return null;
            }

            return action;
        }

        private SubscribeWaitingResponseDelegate GetWaitingCallback(int requestId)
        {
            if (!_waitingForResponseCallbacks.TryGetValue(requestId, out var action))
            {
                return null;
            }

            _waitingForResponseCallbacks.Remove(requestId);
            return action;
        }

        public INetDataHeader ReadHeader(NetDataReader reader)
        {
            var hashOfHeaderName = reader.GetULong();
            if (hashOfHeaderName.Equals("4698292977389273193"))
            {
                int a = 0;
                a++;
            }
            
            if (_headerConstructors.ContainsKey(hashOfHeaderName))
            {
                var header = _headerConstructors[hashOfHeaderName].Invoke();
                header.Deserialize(reader);
                return header;
            }
            else
            {
                return new RequestHeader();
            }

        }

        public void ReadPacket(NetDataReader reader, NetPlayer player)
        {
            var header = ReadHeader(reader);
            if (header.NetType == ENetType.REQUEST || header.NetType == ENetType.RESPONSE)
            {
                var action = GetIncomeRequestCallback(header.NetCommand);
                action?.Invoke(header, reader, player);
            }
            else
            {
                // one way data
                if (header.RequestId == 0)
                {
                    var action = GetIncomeRequestCallback(header.NetCommand);
                    action?.Invoke(header, reader, player);
                }
                else // round trip data
                {
                    var waitingCallback = GetWaitingCallback(header.RequestId);
                    waitingCallback?.Invoke((TWaitingResponseHeader) header, reader, player);
                }
            }
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
        public void Subscribe<TNetRequest>(ENetCommand command, Action<TNetRequest, NetPlayer> onReceive) where TNetRequest : INetData
        {
            _incomeRequestCallbacks[command] = (header, reader, player) =>
            {
                var request = MessagePackSerializer.Deserialize<TNetRequest>(reader.GetBytesWithLength());
                request.Header = header;
                onReceive.Invoke(request, player);
            };
        }

        public void SubscribeWaitingRequest<TNetResponse>(INetData request, Action<TNetResponse, NetPlayer> onSuccess, Action<int> onError, Action<TNetResponse, NetPlayer> onFinally) where TNetResponse : INetData
        {
            _waitingForResponseCallbacks[request.Header.RequestId] = (header, reader, player) =>
            {
                var response = MessagePackSerializer.Deserialize<TNetResponse>(reader.GetBytesWithLength());
                response.Header = header;
                if (header.Error == 0)
                {
                    onSuccess?.Invoke(response, player);
                }
                else
                {
                    onError?.Invoke(header.Error);
                }

                onFinally?.Invoke(response, player);
            };
        }

        public void WritePack(INetData packet, NetDataWriter writer)
        {
            writer.Put(HashName.GetHash(packet.Header.GetType()));
            packet.Serialize(writer);
        }
    }
}
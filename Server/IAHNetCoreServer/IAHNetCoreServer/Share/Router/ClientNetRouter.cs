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
    public class ClientNetRouter
    {
        private delegate void SubscribeIncomeRequestDelegate(RequestHeader header, NetDataReader reader, NetPlayer player);

        private delegate void SubscribeIncomeResponseDelegate(ResponseHeader header, NetDataReader reader, NetPlayer player);

        private delegate void SubscribeWaitingResponseDelegate(ResponseHeader header, NetDataReader reader, NetPlayer player);

        // subscribe with ENetCommand
        private readonly Dictionary<ENetCommand, SubscribeIncomeRequestDelegate>  _incomeRequestCallbacks  = new Dictionary<ENetCommand, SubscribeIncomeRequestDelegate>();
        private readonly Dictionary<ENetCommand, SubscribeIncomeResponseDelegate> _incomeResponseCallbacks = new Dictionary<ENetCommand, SubscribeIncomeResponseDelegate>();

        // subscribe with Request Id
        private readonly Dictionary<int, SubscribeWaitingResponseDelegate> _waitingForResponseCallbacks = new Dictionary<int, SubscribeWaitingResponseDelegate>();

        private readonly Dictionary<ulong, Func<INetDataHeader>> _headerConstructors = new Dictionary<ulong, Func<INetDataHeader>>();

        public Action<Request> OnWaitingTimeOutEvent;

        public void RegisterHeader<T>(Func<T> headerConstructor) where T : INetDataHeader, new()
        {
            var t = typeof(T);
            var hash = HashName.GetHash(t);
            _headerConstructors[hash] = () => headerConstructor.Invoke();
        }

        private SubscribeIncomeRequestDelegate GetIncomeRequestCallback(ENetCommand netCommand)
        {
            if (!_incomeRequestCallbacks.TryGetValue(netCommand, out var action))
            {
                return null;
            }

            return action;
        }

        private SubscribeIncomeResponseDelegate GetIncomeResponseCallback(ENetCommand netCommand)
        {
            if (!_incomeResponseCallbacks.TryGetValue(netCommand, out var action))
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

        public void ReadPacket(NetDataReader reader, NetPlayer player)
        {
            var hash = reader.GetULong();
            var header = _headerConstructors[hash].Invoke();
            header.Deserialize(reader);

            // one way data
            if (header.RequestId == 0)
            {
                if (header is RequestHeader requestHeader)
                {
                    var action = GetIncomeRequestCallback(requestHeader.NetCommand);
                    action?.Invoke(requestHeader, reader, player);
                }
                else if (header is ResponseHeader responseHeader)
                {
                    var action = GetIncomeResponseCallback(header.NetCommand);
                    action?.Invoke(responseHeader, reader, player);
                }
                else
                {
                    // todo networkV2 unknown header 
                }
            }
            else // round trip data
            {
                var waitingCallback = GetWaitingCallback(header.RequestId);
                waitingCallback?.Invoke((ResponseHeader) header, reader, player);
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
        public void SubscribeIncomeRequest<TNetRequest>(ENetCommand command, Action<TNetRequest, NetPlayer> onReceive) where TNetRequest : Request
        {
            _incomeRequestCallbacks[command] = (header, reader, player) =>
            {
                var request = MessagePackSerializer.Deserialize<TNetRequest>(reader.GetBytesWithLength());
                request.Header = header;
                onReceive.Invoke(request, player);
            };
        }
        
        /// <summary>
        /// Register and subscribe to packet receive event (with userData)
        /// </summary>
        /// <param name="command">command id</param>
        /// <param name="onReceive">event that will be called when packet deserialized with ReadPacket method</param>
        public void SubscribeIncomeResponse<TNetResponse>(ENetCommand command, Action<TNetResponse, NetPlayer> onReceive) where TNetResponse : Response
        {
            _incomeResponseCallbacks[command] = (header, reader, player) =>
            {
                var response = MessagePackSerializer.Deserialize<TNetResponse>(reader.GetBytesWithLength());
                response.Header = header;
                onReceive.Invoke(response, player);
            };
        }

        public void SubscribeRequest<TNetResponse>(Request request, Action<Request, TNetResponse, NetPlayer> onSuccess, Action<int> onError, Action<Request, TNetResponse, NetPlayer> onFinally) where TNetResponse : Response
        {
            _waitingForResponseCallbacks[request.Header.RequestId] = (header, reader, player) =>
            {
                var response = MessagePackSerializer.Deserialize<TNetResponse>(reader.GetBytesWithLength());
                response.Header = header;
                if (header.Error == 0)
                {
                    onSuccess?.Invoke(request, response, player);
                }
                else
                {
                    onError?.Invoke(header.Error);
                }

                onFinally?.Invoke(request, response, player);
            };
        }
        
        public void WriteHash(INetDataHeader header, NetDataWriter writer)
        {
            writer.Put(HashName.GetHash(header.GetType()));
        }
        
        public void WritePack<T>(INetData<T> packet, NetDataWriter writer) where T : INetDataHeader
        {
            writer.Put(HashName.GetHash(packet.Header.GetType()));
            packet.Serialize(writer);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using LiteNetLib.Utils;
using MessagePack;
using NLog;
using SourceShare.Share.NetworkV2;
using SourceShare.Share.NetworkV2.Router;
using SourceShare.Share.NetworkV2.TransportData.Base;
using SourceShare.Share.NetworkV2.TransportData.Define;
using SourceShare.Share.NetworkV2.TransportData.Header;

namespace IAHNetCoreServer.NetworkV2.Server
{
    public class NetRouter<T> where T : NetPlayer
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly Dictionary<ulong, Func<INetDataHeader>> _headerConstructors = new Dictionary<ulong, Func<INetDataHeader>>();

        // subscribe with ENetCommand
        private readonly Dictionary<int, SubscribeIncomeDelegate> _incomeRequestCallbacks = new Dictionary<int, SubscribeIncomeDelegate>();

        // subscribe with Request Id
        private readonly Dictionary<int, SubscribeWaitingResponseDelegate> _waitingForResponseCallbacks = new Dictionary<int, SubscribeWaitingResponseDelegate>();


        private readonly ITimeOutChecker _timeOutChecker;

        private readonly string _name;

        public NetRouter(string name, ITimeOutChecker timeOutChecker)
        {
            _name = name;
            _timeOutChecker = timeOutChecker;
        }

        public void RegisterHeader<H>(Func<H> headerConstructor) where H : INetDataHeader, new()
        {
            var t    = typeof(H);
            var hash = HashName.GetHash(t);
            _headerConstructors[hash] = () => headerConstructor.Invoke();
        }

        private SubscribeIncomeDelegate GetIncomeRequestCallback(int netCommand)
        {
            if (!_incomeRequestCallbacks.TryGetValue(netCommand, out var action)) return null;

            return action;
        }

        private SubscribeWaitingResponseDelegate GetWaitingCallback(int requestId)
        {
            if (!_waitingForResponseCallbacks.TryGetValue(requestId, out var action)) return null;

            _waitingForResponseCallbacks.Remove(requestId);
            return action;
        }

        public INetDataHeader ReadHeader(NetDataReader reader)
        {
            var hashOfHeaderName = reader.GetULong();
            if (_headerConstructors.ContainsKey(hashOfHeaderName))
            {
                var header = _headerConstructors[hashOfHeaderName].Invoke();
                header.Deserialize(reader);
                return header;
            }

            return new RequestHeader();
        }

        private async Task<bool> ReadPacket(NetDataReader reader, T player)
        {
            var header = ReadHeader(reader);
#if DEBUG_NETWORK_V2
            Debugger.Write($"[Net] Receive <{player.PlayerId}> {header.NetType} >> {Debugger.FindConstName<NetAPICommand>(header.NetCommand)}");
#endif
            if (header.NetType == ENetType.REQUEST || header.NetType == ENetType.MESSAGE)
            {
                var action = GetIncomeRequestCallback(header.NetCommand);
                if (action != null)
                {
                    await action.Invoke(header, reader, player);
                    return true;
                }
                else
                {
                    Logger.Warn($"Not implement handler of request-message: router={_name} NetType={header.NetType}, command={header.NetCommand}");
                    return false;
                }
            }
            else
            {
                // one way data
                if (header.RequestId == 0)
                {
                    var action = GetIncomeRequestCallback(header.NetCommand);
                    if (action != null)
                    {
                        await action.Invoke(header, reader, player);
                        return true;
                    }
                    else
                    {
                        Logger.Warn($"Not implement handler of request: router={_name} NetType={header.NetType}, command={header.NetCommand}");
                        return false;
                    }
                }
                else // round trip data
                {
                    var waitingCallback = GetWaitingCallback(header.RequestId);
                    if (waitingCallback != null)
                    {
                        waitingCallback.Invoke((ResponseHeader) header, reader, player);
                        return true;
                    }
                    else
                    {
                        Logger.Warn($"Not implement handler of response: router={_name} NetType={header.NetType}, command={header.NetCommand}");
                        return false;
                    }
                }
            }
        }

        /// <summary>
        ///     Reads all available data from NetDataReader and calls OnReceive delegates
        /// </summary>
        /// <param name="reader">NetDataReader with packets data</param>
        /// <param name="player">Argument that passed to OnReceivedEvent</param>
        /// <exception cref="ParseException">Malformed packet</exception>
        public async Task ReadAllPackets(NetDataReader reader, T player)
        {
            bool doNextPacket = true;
            while (reader.AvailableBytes > 0 && doNextPacket)
            {
                try
                {
                    doNextPacket = await ReadPacket(reader, player); // may be wrong net structure or unlistened packet, ignore other packet
                }
                catch (Exception e)
                {
                    doNextPacket = false;
                    Logger.Error(e, $"Handler error, user={player.PlayerId}");
                }
            }
        }

        /// <summary>
        ///     Register and subscribe to packet receive event (with userData)
        /// </summary>
        /// <param name="command">command id</param>
        /// <param name="onReceive">event that will be called when packet deserialized with ReadPacket method</param>
        public void Subscribe<TNetRequest>(int command, Func<TNetRequest, T, Task<INetData>> onReceive) where TNetRequest : INetData
        {
            _incomeRequestCallbacks[command] = async (header, reader, player) =>
                                               {
                                                   var request = MessagePackSerializer.Deserialize<TNetRequest>(reader.GetBytesWithLength());
                                                   request.Header = header;
                                                   return await onReceive.Invoke(request, player);
                                               };
        }

        private void SubscribeWaitingRequest<TNetResponse>(INetData request, Action<TNetResponse, T> onSuccess, Action<int> onError, Action<TNetResponse, NetPlayer> onFinally, float timeOut = 0) where TNetResponse : INetData
        {
            var requestId = GenID();
            request.Header.RequestId = requestId;
            _waitingForResponseCallbacks[requestId] = (header, reader, player) =>
                                                      {
                                                          var response = MessagePackSerializer.Deserialize<TNetResponse>(reader.GetBytesWithLength());
                                                          response.Header = header;
                                                          if (header.Error == 0)
                                                              onSuccess?.Invoke(response, player);
                                                          else
                                                              onError?.Invoke(header.Error);

                                                          onFinally?.Invoke(response, player);
                                                      };

            if (_timeOutChecker != null && timeOut > 0f) _timeOutChecker.Add(request);
        }

        public void SendRequest<TNetResponse>(T destPlayer, INetData request, Action<TNetResponse, NetPlayer> onSuccess, Action<int> onError) where TNetResponse : INetData
        {
            SendRequest<TNetResponse>(destPlayer, request, onSuccess, onError, null, 0);
        }

        public void SendRequest<TNetResponse>(T destPlayer, INetData request, Action<TNetResponse, NetPlayer> onSuccess, Action<int> onError, Action<TNetResponse, NetPlayer> onFinally) where TNetResponse : INetData
        {
            SendRequest<TNetResponse>(destPlayer, request, onSuccess, onError, onFinally, 0);
        }

        public void SendRequest<TNetResponse>(T destPlayer, INetData request, Action<TNetResponse, NetPlayer> onSuccess, Action<int> onError, Action<TNetResponse, NetPlayer> onFinally, int timeOut) where TNetResponse : INetData
        {
            if (onSuccess != null || onError != null || onFinally != null)
                SubscribeWaitingRequest(request, onSuccess, onError, onFinally, timeOut);
            destPlayer.Send(request);
        }

    #region STATIC FUNCTION: auto gen id

        private static volatile int IDMaker = int.MaxValue - 1;

        private static int GenID()
        {
            var id = Interlocked.Increment(ref IDMaker);
            if (id == int.MaxValue)
                Interlocked.Exchange(ref IDMaker, 0);
            return id;
        }

    #endregion

        private delegate Task<INetData> SubscribeIncomeDelegate(INetDataHeader header, NetDataReader reader, T player);

        private delegate void SubscribeWaitingResponseDelegate(ResponseHeader header, NetDataReader reader, T player);
    }
}
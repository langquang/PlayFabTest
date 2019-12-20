using System.Collections.Generic;
using SourceShare.Share.NetworkV2.Base;
using SourceShare.Share.TransportData.Base;
using SourceShare.Share.TransportData.Misc;
using SourceShare.Share.Utils;

namespace IAHNetCoreServer.Server
{
    public class TimeOutChecker : ITimeOutChecker
    {
        private readonly Dictionary<int, RequestTimeOut> _requestsTimeOut = new Dictionary<int, RequestTimeOut>();
        private readonly IReceiveNetDataHandler _handler;

        private int _currentSeconds;

        public TimeOutChecker(IReceiveNetDataHandler handler)
        {
            _handler = handler;
        }

        public void Add(INetData request)
        {
            _requestsTimeOut.Add(request.Header.RequestId, new RequestTimeOut() {command = request.Header.NetCommand, TimeSend = Timer.GetCurrentSeconds()});
        }

        public void Update()
        {
            if (_requestsTimeOut.Count == 0)
                return;

            _currentSeconds = Timer.GetCurrentSeconds();
            foreach (var requestTimeOut in _requestsTimeOut.Values)
            {
                if (_currentSeconds > requestTimeOut.TimeSend + requestTimeOut.TimeOut)
                {
                    _handler.OnTimeRequestTimeOut(requestTimeOut);
                    break;
                }
            }
        }
    }
}
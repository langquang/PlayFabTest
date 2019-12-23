using System.Collections.Generic;
using SourceShare.Share.NetworkV2.Base;
using SourceShare.Share.NetworkV2.TransportData.Base;
using SourceShare.Share.NetworkV2.TransportData.Misc;

namespace UnityClientLib.NetworkV2
{
    public class TimeOutChecker : ITimeOutChecker
    {
        private readonly Dictionary<int, RequestTimeOut> _requestsTimeOut = new Dictionary<int, RequestTimeOut>();
        private readonly IReceiveNetDataHandler          _handler;

        private float _currentSeconds;

        public TimeOutChecker(IReceiveNetDataHandler handler)
        {
            _handler = handler;
        }

        public void Add(INetData request)
        {
            _requestsTimeOut.Add(request.Header.RequestId, new RequestTimeOut() {command = request.Header.NetCommand, TimeSend = (int) UnityEngine.Time.time});
        }

        public void Update()
        {
            if (_requestsTimeOut.Count == 0)
                return;

            _currentSeconds = (int) UnityEngine.Time.time;
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
using SourceShare.Share.TransportData.Misc;

namespace SourceShare.Share.NetworkV2.Base
{
    public interface IReceiveNetDataHandler
    {
        void OnTimeRequestTimeOut(RequestTimeOut requestTimeOut);
    }
}
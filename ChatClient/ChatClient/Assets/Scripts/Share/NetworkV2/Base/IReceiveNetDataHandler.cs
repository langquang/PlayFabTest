using Share.NetworkV2.TransportData.Misc;

namespace Share.NetworkV2.Base
{
    public interface IReceiveNetDataHandler
    {
        void OnTimeRequestTimeOut(RequestTimeOut requestTimeOut);
    }
}
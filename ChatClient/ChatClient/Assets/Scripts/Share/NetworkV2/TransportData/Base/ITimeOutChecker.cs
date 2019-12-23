namespace SourceShare.Share.NetworkV2.TransportData.Base
{
    public interface ITimeOutChecker
    {
        void Add(INetData timeOut);
        void Update();
    }
}
namespace SourceShare.Share.TransportData.Base
{
    public interface ITimeOutChecker
    {
        void Add(INetData timeOut);
        void Update();
    }
}
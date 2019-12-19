using IAHNetCoreServer.Share.TransportData.Define;
using IAHNetCoreServer.Share.TransportData.Header;
using LiteNetLib.Utils;

namespace IAHNetCoreServer.Share.TransportData.Base
{
    public abstract class Response : INetData<ResponseHeader>
    {
        public ResponseHeader Header { get; set; }

        protected Response()
        {
        }

        protected Response(Request request, int errorCode)
        {
            Header = new ResponseHeader(request.Header, errorCode);
        }

        protected Response(ResponseHeader header)
        {
            Header = header;
        }

        protected Response(ENetType netType, ENetCommand command, int errorCode)
        {
            Header = new ResponseHeader(netType, command, errorCode);
        }

        public virtual void Serialize(NetDataWriter writer)
        {
            Header.Serialize(writer);
        }

        public void SetErrorCode(int errorCode)
        {
            Header.Error = errorCode;
        }
    }
}
using IAHNetCoreServer.Share.NetworkV2;
using IAHNetCoreServer.Share.Router;
using IAHNetCoreServer.Share.TransportData;
using IAHNetCoreServer.Share.TransportData.Base;
using IAHNetCoreServer.Share.TransportData.Define;
using IAHNetCoreServer.Share.TransportData.Header;

namespace IAHNetCoreServer.Logic.Client.ResponseHandler
{
    public class ClientResponseHandler
    {
        private ClientNetRouter _router;

        public ClientResponseHandler()
        {
            _router = new ClientNetRouter();

            // Register headers
            _router.RegisterHeader<RequestHeader>(() => new RequestHeader());
            _router.RegisterHeader<ResponseHeader>(() => new ResponseHeader());

            // Subscribe income Handler
            _router.SubscribeIncomeRequest<TestRequest>(ENetCommand.TEST_REQUEST, OnReceiveTestRequest);
            _router.SubscribeIncomeResponse<TestResponse>(ENetCommand.TEST_REQUEST, OnReceiveTestResponse);

            // Subscribe waiting for Response
            _router.SubscribeRequest<TestResponse>(new TestRequest("Test msg from client"), OnSendTestSuccess, OnSendTestError, OnSendTestFinally);
        }

        public void OnReceiveTestRequest(TestRequest request, NetPlayer player)
        {
        }

        public void OnReceiveTestResponse(TestResponse response, NetPlayer player)
        {
        }

        public void OnSendTestSuccess(Request request, TestResponse response, NetPlayer player)
        {
        }

        public void OnSendTestError(int error)
        {
        }

        public void OnSendTestFinally(Request request, TestResponse response, NetPlayer player)
        {
        }
    }
}
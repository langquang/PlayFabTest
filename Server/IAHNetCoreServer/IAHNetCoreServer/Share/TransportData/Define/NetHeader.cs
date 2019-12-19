namespace IAHNetCoreServer.Share.TransportData.Define
{
    public enum ENetType
    {
        REQUEST = 1,
        MESSAGE = 2
    }

    public enum ENetCommand
    {
        // Request
        LOGIN_REQUEST = 1,
        TEST_REQUEST  = 1_000_000,

        // Message
        TEST_MESSAGE = 1
    }
}
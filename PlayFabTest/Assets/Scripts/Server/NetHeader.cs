namespace IAHNetCoreServer.Server
{
    public enum ENetType
    {
        REQUEST = 1,
        MESSAGE = 2
    }

    public enum ENetCommand
    {
        // Request
        TEST_REQUEST = 1,

        // Message
        TEST_MESSAGE = 1,
    }
}
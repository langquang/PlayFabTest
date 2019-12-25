namespace SourceShare.Share.NetRequest.Config
{
    public class NetAPICommand
    {
        // Request
        public const int LOGIN                     = 1;
        public const int CREATE_MASTER_ACCOUNT     = 2;
        public const int CREATE_NODE_ACCOUNT       = 3;
        public const int CHECK_CREATE_NODE_ACCOUNT = 4;
        public const int CHANGE_DISPLAY_NAME       = 5;

        public const int TEST_REQUEST = 1_000_000;

        // Message
        public const int TEST_MESSAGE = 1;
    }
}
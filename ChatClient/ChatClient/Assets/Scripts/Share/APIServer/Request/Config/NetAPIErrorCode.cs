namespace Share.APIServer.Request.Config
{
    public class NetAPIErrorCode
    {
        public const int COMMON_ERROR                           = 1;
        public const int FATAL_ERROR                            = 2;
        public const int INTERNAL_NETWORK_ERROR                 = 3;
        public const int ALREADY_A_MASTER_ACCOUNT               = 4;
        public const int ALREADY_A_NODE_ACCOUNT                 = 5;
        public const int ALREADY_CREATED_ACCOUNT_IN_THIS_SERVER = 6;
        public const int WRONG_SERVER                           = 7;
        public const int WRONG_MASTER_ACCOUNT                   = 8;
        public const int WRONG_REQUEST                          = 9;
    }
}
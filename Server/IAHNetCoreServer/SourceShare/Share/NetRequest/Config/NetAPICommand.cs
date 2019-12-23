using System.ComponentModel;

namespace SourceShare.Share.TransportData.define
{
    public class NetAPICommand
    {
        // Request
        [Description("My Property")] public const int LOGIN_REQUEST = 1;

        [Description("My Property")] public const int TEST_REQUEST = 1_000_000;

        // Message
        [Description("My Property")] public const int TEST_MESSAGE = 1;
    }
}
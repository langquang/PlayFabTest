using System;

namespace SourceShare.Share.NetworkV2.Utils
{
    public class Timer
    {
        public static int GetCurrentSeconds()
        {
            return DateTime.Now.Millisecond;
        }
    }
}
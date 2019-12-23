using System;

namespace SourceShare.Share.Utils
{
    public class Timer
    {
        public static int GetCurrentSeconds()
        {
            return DateTime.Now.Millisecond;
        }
    }
}
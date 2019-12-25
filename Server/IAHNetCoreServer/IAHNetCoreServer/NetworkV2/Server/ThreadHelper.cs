using System.Threading;

namespace NetworkV2.Server
{
    public class ThreadHelper
    {
        private static int threadIndex = 1;
        public static string GetCurrentThreadName(string initName)
        {
            if (string.IsNullOrEmpty(Thread.CurrentThread.Name))
            {
                if(!string.IsNullOrEmpty(initName))
                    Thread.CurrentThread.Name = initName;
                else 
                    Thread.CurrentThread.Name = $"Thread_{threadIndex}";
            }

            return Thread.CurrentThread.Name;
        }
    }
}
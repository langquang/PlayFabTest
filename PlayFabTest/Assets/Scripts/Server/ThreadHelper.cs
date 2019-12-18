using System.Threading;

namespace IAHNetCoreServer.Server
{
    public class ThreadHelper
    {
        public static string GetCurrentThreadName(string initName)
        {
            if (string.IsNullOrEmpty(Thread.CurrentThread.Name))
            {
                Thread.CurrentThread.Name = initName;
            }

            return Thread.CurrentThread.Name;
        }
    }
}
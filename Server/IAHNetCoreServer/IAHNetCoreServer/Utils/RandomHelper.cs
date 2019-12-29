using System.IO;

namespace IAHNetCoreServer.Utils
{
    public class RandomHelper
    {
        /// <summary>
        /// Gen a session ticket
        /// </summary>
        /// <returns></returns>
        public static string RandomString()
        {
            return Path.GetRandomFileName();
        }
    }
}
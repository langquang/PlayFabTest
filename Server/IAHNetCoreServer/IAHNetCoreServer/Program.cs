using System;
using System.Threading;
using IAHNetCoreServer.Server;

namespace IAHNetCoreServer
{
    class Program
    {
        private static int    port = 8000;
        private static string key  = "ButinABC";

        static void Main(string[] args)
        {
            Console.WriteLine($"Start Server with Thread: {ThreadHelper.GetCurrentThreadName("Main")}");
            Console.WriteLine($"Creating a server with port:{port}");
            NetServer netServer = new NetServer("Server", new ServerHandler());
            netServer.Start(port, key);
            Console.ReadKey();
        }
    }
}
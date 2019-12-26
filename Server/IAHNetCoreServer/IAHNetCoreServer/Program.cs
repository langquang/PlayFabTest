using System;
using System.Threading;
using IAHNetCoreServer.Logic.Server.RequestHandlers;
using IAHNetCoreServer.Logic.Server.SGPlayFab;
using MessagePack;
using MessagePack.Resolvers;
using MessagePack.Unity;
using MessagePack.Unity.Extension;
using NetworkV2.Base;
using NetworkV2.Server;
using PlayFabCustom.Models;
using SourceShare.Share.NetworkV2;

namespace IAHNetCoreServer
{
    class Program
    {
        private static int    port = 8000;
        private static string key  = "ButinABC";

        static void Main(string[] args)
        {
            SetupMessagePack();
            SetupPlayFab();

            Console.WriteLine($"Start Server with Thread: {ThreadHelper.GetCurrentThreadName("Main")}");
            Console.WriteLine($"Creating a server with port:{port}");
            NetServer<DataPlayer> netServer = new NetServer<DataPlayer>("Server", new EntryHandler());
            netServer.Start(port, key);

            int workerThreads, completionPortThreads;
            ThreadPool.GetMaxThreads(out workerThreads, out completionPortThreads);
            Console.WriteLine($"workerThreads={workerThreads},completionPortThreads={completionPortThreads}");
            
            // bool _quitFlag = false;
            // while (!_quitFlag)
            // {
            //     var keyInfo = Console.ReadKey();
            //     _quitFlag = keyInfo.Key == ConsoleKey.C
            //                 && keyInfo.Modifiers == ConsoleModifiers.Control;
            // }
            
        }

        static void SetupMessagePack()
        {
            // set extensions to default resolver.
            var resolver = CompositeResolver.Create(
                // enable extension packages first
                UnityBlitResolver.Instance,
                UnityResolver.Instance,
                // finally use standard(default) resolver
                StandardResolver.Instance
            );
            var options = MessagePackSerializerOptions.Standard.WithResolver(resolver);

            // pass options to every time or set as default
            MessagePackSerializer.DefaultOptions = options;
        }

        static void SetupPlayFab()
        {
            PFDriver.Setup();
        }
    }
}
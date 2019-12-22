using System;
using System.Threading;
using IAHNetCoreServer.Logic.Server.RequestHandlers;
using IAHNetCoreServer.Server;
using IAHNetCoreServer.Share.NetworkV2;
using MessagePack;
using MessagePack.Resolvers;
using UnityEngine;

namespace IAHNetCoreServer
{
    class Program
    {
        private static int    port = 8000;
        private static string key  = "ButinABC";

        static void Main(string[] args)
        {
            ConfigMessagePack();
            
            Console.WriteLine($"Start Server with Thread: {ThreadHelper.GetCurrentThreadName("Main")}");
            Console.WriteLine($"Creating a server with port:{port}");
            NetServer netServer = new NetServer("Server", new EntryHandler());
            netServer.Start(port, key);

            bool _quitFlag = false;
            while(!_quitFlag)
            {
                var keyInfo = Console.ReadKey();
                _quitFlag = keyInfo.Key == ConsoleKey.C
                            && keyInfo.Modifiers == ConsoleModifiers.Control;
            }
        }

        static void ConfigMessagePack()
        {
            // set extensions to default resolver.
            var resolver = CompositeResolver.Create(
                // enable extension packages first
                MessagePack.Unity.Extension.UnityBlitResolver.Instance,
                MessagePack.Unity.UnityResolver.Instance,
                // finally use standard(default) resolver
                StandardResolver.Instance
            );
            var options = MessagePackSerializerOptions.Standard.WithResolver(resolver);

            // pass options to every time or set as default
            MessagePackSerializer.DefaultOptions = options;
        }
    }
}
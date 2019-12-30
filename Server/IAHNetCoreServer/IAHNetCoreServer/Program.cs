using System;
using System.Threading;
using System.Threading.Tasks;
using IAHNetCoreServer.Logic.Server.RequestHandlers;
using IAHNetCoreServer.Logic.Server.SGPlayFab;
using MessagePack;
using MessagePack.Resolvers;
using MessagePack.Unity;
using MessagePack.Unity.Extension;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NetworkV2.Base;
using NLog;
using NLog.Extensions.Hosting;
using NLog.Extensions.Logging;
using PlayFabCustom.Models;
using ILogger = Microsoft.Extensions.Logging.ILogger;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace IAHNetCoreServer
{
    class Program
    {
        public static int    port = 8000;
        public static string key  = "ButinABC";


        private static Logger           logger = LogManager.GetCurrentClassLogger();
        public static NetServer<DataPlayer> netServer;


        static async Task Main(string[] args)
        {
            SetupMessagePack();
            SetupPlayFab();
            logger.Info("Try to get PlayFab Catalog ....");
            await PFCatalog.Init();
            AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;
            
            var hostBuilder = new HostBuilder()
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddLogging(loggingBuilder =>
                    {
                        loggingBuilder.ClearProviders();
                        loggingBuilder.SetMinimumLevel(LogLevel.Trace);
                        loggingBuilder.AddNLog();
                    });
                    services.AddHostedService<LifetimeEventsHostedService>();
                })
                .UseNLog()
                .UseConsoleLifetime();
                // .Build();
            
            await hostBuilder.RunConsoleAsync();

            // Console.WriteLine("The host container has terminated. Press ANY key to exit the console.");
            // Console.ReadKey();
        }

        private static void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            logger.Info("================== STOPPING SERVER ================");
            netServer.Stop();
            Thread.Sleep(100);
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
    
    internal class LifetimeEventsHostedService : IHostedService, IDisposable
    {
        private readonly ILogger                  _logger;
        private readonly IHostApplicationLifetime _appLifetime;

        public LifetimeEventsHostedService(
            ILogger<LifetimeEventsHostedService> logger, 
            IHostApplicationLifetime appLifetime)
        {
            _logger = logger;
            _appLifetime = appLifetime;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting daemon: ");
            
            _appLifetime.ApplicationStarted.Register(OnStarted);
            _appLifetime.ApplicationStopping.Register(OnStopping);
            _appLifetime.ApplicationStopped.Register(OnStopped);
            
            Program.netServer = new NetServer<DataPlayer>("Server", new EntryHandler());
            Program.netServer.Start(Program.port, Program.key);
            
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("stop daemon: ");

            return Task.CompletedTask;
        }

        private void OnStarted()
        {
            _logger.LogInformation("OnStarted has been called.");

            // Perform post-startup activities here
        }

        private void OnStopping()
        {
            _logger.LogInformation("OnStopping has been called.");

            // Perform on-stopping activities here
        }

        private void OnStopped()
        {
            _logger.LogInformation("OnStopped has been called.");

            // Perform post-stopped activities here
            Program.netServer.Stop();
        }
        
        public void Dispose()
        {
            _logger.LogInformation("Disposing....");
        }
    }
}
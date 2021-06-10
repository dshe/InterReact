using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace InterReact.UnitTests.Experimental
{
    public class Test_Logging : BaseUnitTest
    {
        public Test_Logging(ITestOutputHelper output) : base(output) { }

        //[Fact]
        public async Task TestLoggingInjection()
        {
            var host = new HostBuilder()
                .ConfigureLogging(builder => builder
                    .AddDebug()
                    .AddEventLog() // Windows EventLog
                    .AddMXLogger(Write)
                    .SetMinimumLevel(LogLevel.Trace))
                .ConfigureServices((context, services) => services
                    //.AddLogging()
                    .AddSingleton<InterReactBuilder>()
                    .AddSingleton<Task<IInterReact>>(providor =>
                        providor.GetRequiredService<InterReactBuilder>().BuildAsync())
                    .AddHostedService<StartupService>(services => new StartupService(services)))
                .Build();

            await host.StartAsync();

            //var app = host.Services.GetService<MyAppRoot>();
            //await app.Start();
            //var app = host.Services.GetRequiredService<Xxx>();
            ;
            //var o = await Builder.BuildAsync();
            //IHostedService
            //AddHostedService

            //var interreact = new InterReactClientBuilder()
            //    .BuildAsync();
        }
    }

    public class StartupService : IHostedService
    {
        private readonly IServiceProvider Providor;
        private readonly ILogger Logger;
        internal IInterReact? Client = null;

        public StartupService(IServiceProvider providor)
        {
            Providor = providor;
            Logger = providor.GetService<ILogger<InterReact>>()!;
        }

        public async Task StartAsync(CancellationToken ct)
        {
            var task = Providor.GetRequiredService<Task<IInterReact>>();
            Client = await task;
        }

        public async Task StopAsync(CancellationToken ct)
        {
            if (Client != null)
                await Client.DisposeAsync().ConfigureAwait(false);
        }
    }

}

using System;
using System.Diagnostics;
using System.Threading.Tasks;
using InterReact;
using InterReact.Messages;
using InterReact.StringEnums;
using Xunit;
using Xunit.Abstractions;
using InterReact.Interfaces;
using System.Collections.Generic;
using System.Reactive.Linq;
using Microsoft.Extensions.Logging;
using MXLogger;

#nullable enable

namespace InterReact.Tests.Utility
{
    // Collection Fixtures: creates a single test context which is shared among multipletest classes

    [CollectionDefinition("SystemTestCollection")]
    public class AnyNameCollection : ICollectionFixture<SystemTestFixture>
    {
        // This class has no code, and is never created. 
        // Its purpose is simply to be the place to apply [CollectionDefinition]
        // and all the ICollectionFixture<> interfaces.
        // Yes, convoluted.
    }

    public sealed class SystemTestFixture : IAsyncLifetime
    {
        public IInterReactClient? Client { get; private set; }

        public SystemTestFixture()
        {
            Debug.WriteLine("ctor");
        }

        public async Task InitializeAsync()
        {
            Debug.WriteLine("init start");
            Client = await new InterReactClientBuilder().BuildAsync().ConfigureAwait(false);
            Debug.WriteLine("init end");
        }
        public void Dispose()
        {
            Debug.WriteLine("dtor");
            Client!.Dispose();
        }

        public async Task DisposeAsync()
        {
            await Task.Delay(0);
        }
    }

    [Collection("SystemTestCollection")]
    public abstract class BaseSystemTest : IDisposable
    {
        protected readonly IInterReactClient Client;
        protected readonly ILoggerFactory LoggerFactory;
        protected readonly ILogger Logger;
        protected int Id;
        private readonly IDisposable subscription;
        protected readonly List<object> Responses = new List<object>();

        protected BaseSystemTest(SystemTestFixture fixture, ITestOutputHelper output)
        {
            if (fixture.Client == null)
                throw new NullReferenceException(nameof(fixture.Client));

            var provider = new MXLoggerProvider(output.WriteLine);
            LoggerFactory = new LoggerFactory(new[] { provider });
            Logger = LoggerFactory.CreateLogger("SystemTest");

            Client = fixture.Client;
            Id = Client.Request.NextId();
            Logger.LogDebug($"BaseSystemTest: Id = {Id}.");
            subscription = Client.Response.Spy(Logger).Subscribe(Responses.Add);
        }

        public void Dispose() => subscription.Dispose();

        protected readonly Contract Stock1
            = new Contract { SecurityType = SecurityType.Stock, Symbol = "IBM", Currency = "USD", Exchange = "SMART" };

        protected readonly Contract Stock2
            = new Contract { SecurityType = SecurityType.Stock, Symbol = "AAPL", Currency = "USD", Exchange = "SMART" };
    }

}

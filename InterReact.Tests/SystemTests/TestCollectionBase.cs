using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Stringification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

// CollectionDefinition - tests do not run in parallel, which keeps logging relevent!

namespace InterReact.SystemTests
{
    public class TestFixture : IAsyncLifetime
    {
        internal readonly DynamicLogger DynamicLogger = new();
        internal IInterReactClient? Client;
        public TestFixture() { }
        public async Task InitializeAsync()
        {
            Client = await new InterReactClientBuilder(DynamicLogger)
                //.SetPort(7497)
                .LogIncomingMessages()
                .BuildAsync().ConfigureAwait(false);
        }
        public async Task DisposeAsync()
        {
            if (Client != null)
                await Client.DisposeAsync().ConfigureAwait(false);
        }
    }

    [CollectionDefinition("Test Collection")]
    public class TestCollection : ICollectionFixture<TestFixture>
    {
        // This class has no code, and is never created.
        // Its purpose is simply to be the place to apply [CollectionDefinition]
        // and all the ICollectionFixture<> interfaces.
    }

    [Collection("Test Collection")]
    [Trait("Category", "SystemTests")]
    public class TestCollectionBase
    {
        protected readonly Action<string> Write;
        protected readonly ILogger Logger;
        protected readonly IInterReactClient Client;
        protected int Id;

        public TestCollectionBase(ITestOutputHelper output, TestFixture fixture)
        {
            Write = output.WriteLine;
            Logger = new LoggerFactory().AddMXLogger(Write, LogLevel.Debug).CreateLogger("Test");
            fixture.DynamicLogger.Add(Logger, true);
            Client = fixture.Client ?? throw new Exception("Client");
            Client.Response.Subscribe(m =>
            {
                var s = new Stringifier(Logger).Stringify(m);
                Write(s);
            });
            Id = Client.Request.GetNextId();
        }

        protected readonly Contract Stock1 =
            new() { SecurityType = SecurityType.Stock, Symbol = "IBM", Currency = "USD", Exchange = "SMART" };

        protected readonly Contract Stock2 =
            new() { SecurityType = SecurityType.Stock, Symbol = "AAPL", Currency = "USD", Exchange = "SMART" };

        protected readonly Contract Forex1 =
            new() { SecurityType = SecurityType.Cash, Symbol = "EUR", Currency = "USD", Exchange = "IDEALPRO" };
    }
}

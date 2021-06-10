using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
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
        internal IInterReact? Client;
        public TestFixture() { }
        public async Task InitializeAsync()
        {
            Client = await new InterReactBuilder(DynamicLogger).BuildAsync().ConfigureAwait(false);
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
        protected readonly IInterReact Client;
        protected int Id;

        public TestCollectionBase(ITestOutputHelper output, TestFixture fixture)
        {
            Write = output.WriteLine;
            Logger = new LoggerFactory().AddMXLogger(Write).CreateLogger("Test");
            fixture.DynamicLogger.Set(Logger);
            Client = fixture.Client ?? throw new Exception("Client");
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

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
        public IInterReactClient Client { get; private set; }

        public SystemTestFixture()
        {
            Debug.WriteLine("ctor");
        }

        public async Task InitializeAsync()
        {
            Debug.WriteLine("init start");
            Client = await InterReactClient.Builder.BuildAsync().ConfigureAwait(false);
            Debug.WriteLine("init end");
        }

        public Task DisposeAsync()
        {
            Debug.WriteLine("dtor");
            Client?.Dispose();
            return Task.CompletedTask;
        }
    }

    [Collection("SystemTestCollection")]
    public abstract class BaseSystemTest : IDisposable
    {
        protected readonly IInterReactClient Client;
        protected readonly Action<string> Write;
        protected int Id;
        private readonly IDisposable subscription;
        protected readonly List<object> Responses = new List<object>();

        protected BaseSystemTest(SystemTestFixture fixture, ITestOutputHelper output)
        {
            Client = fixture.Client;
            Write = output.WriteLine;
            Id = Client.Request.NextId();
            Write($"BaseSystemTest: Id = {Id}.");
            subscription = Client.Response.Spy(Write).Subscribe(Responses.Add);
        }

        public void Dispose() => subscription.Dispose();

        protected readonly Contract Stock1
            = new Contract { SecurityType = SecurityType.Stock, Symbol = "IBM", Currency = "USD", Exchange = "SMART" };

        protected readonly Contract Stock2
            = new Contract { SecurityType = SecurityType.Stock, Symbol = "AAPL", Currency = "USD", Exchange = "SMART" };
    }

}

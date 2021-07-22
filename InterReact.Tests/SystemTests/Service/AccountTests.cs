using InterReact;
using InterReact.SystemTests;
using Microsoft.Extensions.Logging;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace InterReact.SystemTests.Service
{
    public class SystemTest : TestCollectionBase //BaseTest
    {
        public SystemTest(ITestOutputHelper output, TestFixture fixture) : base(output, fixture) { }

        [Fact]
        public async Task T01_ManagedAccounts()
        {
            var accounts = await Client.Services.ManagedAccountsObservable;
            Assert.NotEmpty(accounts);
        }

        [Fact]
        public async Task T02_AccountPositions()
        {
            var observable = Client.Services.CreatePositionsObservable();

            var list = await observable.ToList();
            // The demo account may or may not have positions.
            Logger.LogInformation("resr");

            if (!Client.Request.Config.IsDemoAccount)
                Assert.NotEmpty(list);
        }

        [Fact]
        public async Task T03_AccountSummary()
        {
            var observable = Client.Services.CreateAccountSummaryObservable();

            var list = await observable.ToList(); 
            Assert.NotEmpty(list);
        }

        [Fact]
        public async Task T04_AccountUpdates()
        {
            var observable = Client.Services.CreateAccountUpdatesObservable();

            //var connection = updates.Connect();

            var list = observable.TakeWhile(x => !(x.Source is AccountUpdateEnd)).ToList().ToTask();

            await list;

            //connection.Dispose();
        }
    }
}

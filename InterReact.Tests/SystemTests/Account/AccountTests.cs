using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using InterReact.Messages;
using InterReact.Tests.Utility;
using Xunit;
using Xunit.Abstractions;

namespace InterReact.Tests.SystemTests.Account
{
    public class SystemTest : BaseSystemTest
    {
        public SystemTest(SystemTestFixture fixture, ITestOutputHelper output) : base(fixture, output) {}

        [Fact]
        public async Task T01_ManagedAccounts()
        {
            var accounts = await Client.Services.ManagedAccountsObservable;
            Assert.NotEmpty(accounts);
        }

        [Fact]
        public async Task T02_AccountPositions()
        {
            var list = await Client.Services.AccountPositionsObservable.ToList();
            // The demo account may or may not have positions.
            if (!Client.Config.IsDemoAccount)
                Assert.NotEmpty(list);
        }

        [Fact]
        public async Task T03_AccountSummary()
        {
            var list = await Client.Services.AccountSummaryObservable.ToList();
            Assert.NotEmpty(list);
        }

        [Fact]
        public async Task T04_AccountUpdates()
        {
            // Results are cached so that all subscribers receive everything.
            var updates = Client.Services.AccountUpdatesConnectableObservable;

            var connection = updates.Connect();

            var list = updates.TakeWhile(x => !(x is AccountUpdateEnd)).ToList().ToTask();

            await list;

            connection.Dispose();
        }
    }
}

using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using InterReact;
using Xunit;
using Xunit.Abstractions;
using InterReact.SystemTests;

namespace SystemTests.Account
{
    public class SystemTest : BaseTest
    {
        public SystemTest(ITestOutputHelper output) : base(output) { }

        [Fact]
        public async Task T01_ManagedAccounts()
        {
            var accounts = await Client.Services.CreateManagedAccountsObservable();
            Assert.NotEmpty(accounts);
        }

        [Fact]
        public async Task T02_AccountPositions()
        {
            var list = await Client.Services.CreateAccountPositionsObservable().ToList();
            // The demo account may or may not have positions.
            if (!Client.Config.IsDemoAccount)
                Assert.NotEmpty(list);
        }

        [Fact]
        public async Task T03_AccountSummary()
        {
            var list = await Client.Services.CreateAccountSummaryObservable().ToList();
            Assert.NotEmpty(list);
        }

        [Fact]
        public async Task T04_AccountUpdates()
        {
            // Results are cached so that all subscribers receive everything.
            var updates = Client.Services.CreateAccountUpdatesConnectableObservable();

            var connection = updates.Connect();

            var list = updates.TakeWhile(x => !(x is AccountUpdateEnd)).ToList().ToTask();

            await list;

            connection.Dispose();
        }
    }
}

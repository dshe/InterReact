using Stringification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace InterReact.SystemTests.Account
{
    public class AccountTest : TestCollectionBase
    {
        public AccountTest(ITestOutputHelper output, TestFixture fixture) : base(output, fixture) { }

        [Fact]
        public async Task T02_AccountPositions()
        {
            IObservable<Union<Position, PositionEnd>> observable = Client.Services.CreatePositionsObservable();

            IList<Position> list = await observable
                .Select(u => u.Source)
                .TakeWhile(o => o is not PositionEnd)
                .OfType<Position>()
                .ToList();

            // The demo account may or may not have positions.
            if (!list.Any())
                Write("no positions!");
        }

        [Fact]
        public async Task T03_AccountSummary()
        {
            IObservable<IHasRequestId> observable = Client
                .Services
                .CreateAccountSummaryObservable();

            IList<IHasRequestId> list = await observable
                .TakeWhile(o => o is not AccountSummaryEnd)
                .ToList();

            foreach (AccountSummary a in list.OfType<AccountSummary>())
                Write(a.Stringify());
        }

        [Fact]
        public async Task T04_AccountUpdates()
        {
            IObservable<Union<AccountValue, PortfolioValue, AccountUpdateTime, AccountUpdateEnd>> observable = Client
                .Services
                .CreateAccountUpdatesObservable();

            IList<object> list = await observable
                .Select(u => u.Source)
                .TakeWhile(o => o is not AccountUpdateEnd)
                .ToList();

            foreach (object o  in list)
                Write(o.Stringify());

        }
    }
}

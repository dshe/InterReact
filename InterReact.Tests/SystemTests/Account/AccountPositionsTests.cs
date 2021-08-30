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
    public class AccountPositionsTest : TestCollectionBase
    {
        public AccountPositionsTest(ITestOutputHelper output, TestFixture fixture) : base(output, fixture) { }

        [Fact]
        public async Task AccountPositions()
        {
            IList<Position> list = await Client
                .Services
                .PositionsObservable
                .TakeWhile(o => o is not PositionEnd)
                .Cast<Position>()
                .ToList();

            // The account may or may not have positions.
            if (!list.Any())
                Write("no positions!");
            foreach (object o in list)
                Write(o.Stringify());
        }
    }
}

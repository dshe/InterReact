using System;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using InterReact.Interfaces;
using InterReact.Messages;
using InterReact.Enums;
using InterReact.Tests.Utility;
using Xunit;
using Xunit.Abstractions;
using System.Reactive.Threading.Tasks;
using System.Threading;
using InterReact.Extensions;
using InterReact.Utility;
using InterReact.StringEnums;
using Microsoft.Extensions.Logging;

namespace InterReact.Tests.SystemTests.MarketData
{
    public class HistoricalDataTests : BaseSystemTest
    {
        public HistoricalDataTests(SystemTestFixture fixture, ITestOutputHelper output) : base(fixture, output) { }

        [Fact]
        public async Task T01_HistoricalData()
        {
            var data = await Client.Services.HistoricalDataObservable(Stock1,
                HistoricalBarSize.OneHour,
                HistoricalDuration.OneDay);

            Logger.LogDebug($"\n\nStart {data.Start}, End: {data.End}\n");
            foreach (var item in data.Bars)
                Logger.LogDebug(item.Stringify());
        }

    }
}


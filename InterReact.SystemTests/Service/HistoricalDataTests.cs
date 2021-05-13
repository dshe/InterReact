using System.Reactive.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using InterReact;
using Stringification;
using InterReact.SystemTests;

namespace SystemTests.MarketData
{
    public class HistoricalDataTests : BaseTest
    {
        public HistoricalDataTests(ITestOutputHelper output) : base(output) { }

        [Fact]
        public async Task T01_HistoricalData()
        {
            var data = await Client.Services.CreateHistoricalDataObservable(Stock1,
                HistoricalBarSize.OneHour,
                HistoricalDuration.OneDay);

            Write($"\n\nStart {data.Start}, End: {data.End}\n");
            foreach (var item in data.Bars)
                Write(item.Stringify());
        }

    }
}


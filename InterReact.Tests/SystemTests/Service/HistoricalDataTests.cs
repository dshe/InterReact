using InterReact;
using InterReact.SystemTests;
using Stringification;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace InterReact.SystemTests.Service
{
    public class HistoricalDataTests : TestCollectionBase
    {
        public HistoricalDataTests(ITestOutputHelper output, TestFixture fixture) : base(output, fixture) { }

        [Fact]
        public async Task T01_HistoricalBarData()
        {
            HistoricalData data = await Client.Services.CreateHistoricalDataObservable(Stock1,
                HistoricalBarSize.OneHour,
                HistoricalDuration.OneDay)
                .OfTypeUnionSource<HistoricalData>();

            Write($"\n\nStart {data.Start}, End: {data.End}\n");
            foreach (var item in data.Bars)
                Write(item.Stringify());
        }

    }
}


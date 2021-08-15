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
            Union<HistoricalData, Alert> union = await Client
                .Services
                .CreateHistoricalDataObservable(StockContract1, HistoricalBarSize.OneHour, HistoricalDuration.OneDay);

            object source = union.Source;

            if (source is Alert alert)
            {
                Write($"Alert: {alert.Message}.");
                return;
            }

            HistoricalData data = (HistoricalData)source;
            
            Write($"\n\nStart {data.Start}, End: {data.End}\n");
            foreach (var item in data.Bars)
                Write(item.Stringify());
        }

    }
}


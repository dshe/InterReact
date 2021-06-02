
/*
namespace InterReact.UnitTests.Utility
{
    public class ContractDataTimeTests : BaseUnitTest
    {
        public ContractDataTimeTests(ITestOutputHelper output) : base(output) { }

        public ContractData ContractData = new ContractData
        {
            TimeZoneId   = "America/New_York",
            TradingHours = "20180101:0900-1215,1315-1700;20180102:0900-1215,1315-1700",
            LiquidHours  = "20180101:0930-1200,1330-1600;20180102:0930-1200,1330-1600"
        };

        [Fact]
        public void T00_TimeZoneTest()
        {
            var cdt = new ContractDataTime(ContractData);
            var tz = DateTimeZoneProviders.Tzdb.GetZoneOrNull(ContractData.TimeZoneId);
            Assert.Equal(tz, cdt.TimeZone);
            Assert.Equal(16, cdt.Events.Count);
        }

        [Fact]
        public void T01_EventsTest()
        {
            var cdt = new ContractDataTime(ContractData);

            var status = cdt.Get(Instant.MinValue);
            Assert.Equal(null, status.Previous);
            Assert.Equal(new LocalDateTime(2018, 1, 1, 9, 0, 0).InZoneStrictly(cdt.TimeZone), status.Next.Time);
            Assert.Equal(ContractTimeStatus.Trading, status.Next.Status);

            var dt = new LocalDateTime(2018, 1, 1, 9, 0, 0).InZoneStrictly(cdt.TimeZone);
            status = cdt.Get(dt.ToInstant());
            Assert.Equal(dt, status.Previous.Time);
            Assert.Equal(ContractTimeStatus.Trading, status.Previous.Status);
            dt = new LocalDateTime(2018, 1, 1, 9, 30, 0).InZoneStrictly(cdt.TimeZone);
            Assert.Equal(dt, status.Next.Time);
            Assert.Equal(ContractTimeStatus.Liquid, status.Next.Status);

            status = cdt.Get(Instant.MaxValue);
            Assert.Equal(new LocalDateTime(2018, 1, 2, 17, 0, 0).InZoneStrictly(cdt.TimeZone), status.Previous.Time);
            Assert.Equal(ContractTimeStatus.Closed, status.Previous.Status);
            Assert.Equal(null, status.Next);
        }

        [Fact]
        public async Task T02_ObservableTest()
        {
            var tz = DateTimeZoneProviders.Tzdb.GetZoneOrNull(ContractData.TimeZoneId);

            var dto = new LocalDateTime(2018, 1, 1, 10, 0, 00).InZoneStrictly(tz).ToInstant().ToDateTimeOffset();
            var scheduler = new HistoricalScheduler(dto); // TestScheduler();

            var cdt = new ContractDataTime(ContractData, scheduler);

            var status = await cdt.ContractTimeObservable.FirstAsync();

            var dt1 = new LocalDateTime(2018, 1, 1, 9, 30, 0).InZoneStrictly(cdt.TimeZone);
            Assert.Equal(dt1, status.Previous.Time);
            Assert.Equal(ContractTimeStatus.Liquid, status.Previous.Status);
            var dt2 = new LocalDateTime(2018, 1, 1, 12, 0, 0).InZoneStrictly(cdt.TimeZone);
            Assert.Equal(dt2, status.Next.Time);
            Assert.Equal(ContractTimeStatus.Trading, status.Next.Status);
        }

        [Fact]
        public async Task T03_ObservableAll()
        {
            var tz = DateTimeZoneProviders.Tzdb.GetZoneOrNull(ContractData.TimeZoneId);
            var scheduler = new HistoricalScheduler();
            var cdt = new ContractDataTime(ContractData, scheduler);
            var task = cdt.ContractTimeObservable.ToList().ToTask();
            scheduler.Start(); // play all
            var list = await task;
            Assert.Equal(cdt.Events.Count + 1, list.Count);
        }
    }
}
*/

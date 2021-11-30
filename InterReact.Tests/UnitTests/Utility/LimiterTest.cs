using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Threading;
using Xunit;
using Xunit.Abstractions;

namespace InterReact.UnitTests.Utility;

public sealed class LimiterTest : UnitTestsBase
{
    public LimiterTest(ITestOutputHelper output) : base(output) { }

    public double Limit(int rate, double duration)
    {
        var limiter = new RingLimiter(Logger, rate);
        var count = 0;
        var start = Stopwatch.GetTimestamp();
        while (((Stopwatch.GetTimestamp() - start) / (double)Stopwatch.Frequency) < duration)
        {
            limiter.Limit(() => Thread.Sleep(1));
            count++;
        }
        var time = (Stopwatch.GetTimestamp() - start) / (double)Stopwatch.Frequency;
        var freq = count / time;
        Logger.LogInformation("{Count} / {Time} => {Freq} ({Rate})", count, time, freq, rate);
        return freq;
    }

    [Fact]
    public void Test()
    {
        double rate;

        //rate = Limit(5, 2);
        //Assert.InRange(rate, 4, 6);

        rate = Limit(50, 2);
        Assert.InRange(rate, 47, 51);

        //rate = Limit(100, 2);
        //Assert.InRange(rate, 98, 101);

        //rate = Limit(200, 2);
        //Assert.InRange(rate, 198, 201);
    }
}

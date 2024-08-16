using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Utility;

public sealed class Limiter(ITestOutputHelper output) : UnitTestBase(output)
{
    public double Limit(int rate, double duration)
    {
        RingLimiter limiter = new(Logger, rate);
        int count = 0;
        long start = Stopwatch.GetTimestamp();
        while ((Stopwatch.GetTimestamp() - start) / (double)Stopwatch.Frequency < duration)
        {
            limiter.Limit(() => Thread.Sleep(0));
            count++;
        }
        double time = (Stopwatch.GetTimestamp() - start) / (double)Stopwatch.Frequency;
        double freq = count / time;
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
        Assert.InRange(rate, 20, 51);

        //rate = Limit(100, 2);
        //Assert.InRange(rate, 98, 101);

        //rate = Limit(200, 2);
        //Assert.InRange(rate, 198, 201);
    }
}

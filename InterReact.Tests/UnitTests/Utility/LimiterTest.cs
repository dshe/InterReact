using InterReact.Utility;
using System.Diagnostics;
using System.Threading;
using Xunit;

namespace InterReact.UnitTests.Utility
{
    public sealed class LimiterTest
    {
        public double Limit(int rate, double duration)
        {
            var limiter = new Limiter(rate);
            var count = 0;
            var start = Stopwatch.GetTimestamp();
            while ((Stopwatch.GetTimestamp() - start) / (double)Stopwatch.Frequency < duration)
            {
                limiter.Limit(() => Thread.Sleep(1));
                count++;
            }
            var time = (Stopwatch.GetTimestamp() - start) / (double)Stopwatch.Frequency;
            var freq = count / time;
            //Logger.LogDebug($"{count} / {time} => {freq} ({rate})");
            return freq;
        }

        //[Fact]
        public void Test()
        {
            var rate = Limit(50, 1);
            Assert.InRange(rate, 47, 51);

            rate = Limit(50, 1.5);
            Assert.InRange(rate, 45, 51);

            rate = Limit(100, .5);
            Assert.InRange(rate, 98, 101);

            rate = Limit(100, 1.5);
            Assert.InRange(rate, 98, 101);

            rate = Limit(50, 4);
            Assert.InRange(rate, 37, 51);
        }
    }
}

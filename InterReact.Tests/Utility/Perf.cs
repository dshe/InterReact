using System;
using System.Diagnostics;

namespace InterReact.Tests.Utility
{
    public class Perf
    {
        private readonly Action<string> Write;

        public Perf(Action<string> write) => Write = write;

        private static double MeasureTicks(Action action, bool prime = true)
        {
            var counter = 1L;
            var sw = new Stopwatch();
            if (prime)
                action();
            sw.Start();
            do
            {
                action();
                counter++;
            } while (sw.ElapsedMilliseconds < 100);
            sw.Stop();
            return sw.ElapsedTicks / (double)counter;
        }

        public void MeasureRate(Action action, string label)
        {
            var frequency = Stopwatch.Frequency / MeasureTicks(action);
            Write($"{frequency,11:####,###} {label}");
        }

        public void MeasureDuration(Action action, long iterations, string label)
        {
            var ticks = (long)(MeasureTicks(action) * iterations);
            var ts = TimeSpan.FromTicks(ticks);
            Write($"{ts} {label}");
        }

    }
}

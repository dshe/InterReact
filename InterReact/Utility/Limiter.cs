using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace InterReact.Utility
{
    public sealed class Limiter
    {
        private readonly List<long> Ring = new();
        private readonly int Rate;
        private int Index;

        internal Limiter(int rate = 0)
        {
            if (rate <= 0)
                return;
            Rate = rate;
            Ring.AddRange(Enumerable.Range(1, rate).Select(i => 0L));
        }

        internal void Limit(Action action)
        {
            lock (Ring)
            {
                if (Rate > 0)
                {
                    Index = (Index == Rate - 1) ? 0 : Index + 1;
                    var wait = 1 + (Ring[Index] - Stopwatch.GetTimestamp()) / (double)Stopwatch.Frequency;
                    if (wait > 0)
                    {
                        //Debug.WriteLine("{0} delay: {1:N0}", index, wait / Stopwatch.Frequency * 1000);
                        Thread.Sleep(Convert.ToInt32(wait * 1000));
                    }
                    Ring[Index] = Stopwatch.GetTimestamp();
                }
                action();
            }
        }
    }
}

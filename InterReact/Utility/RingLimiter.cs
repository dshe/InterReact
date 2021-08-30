using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Diagnostics;
using System.Threading;

namespace InterReact
{
    public sealed class RingLimiter
    {
        private readonly int Rate;
        private readonly ILogger Logger;
        private readonly long[] Ring;
        private int Index;

        internal RingLimiter(int rate = 0, ILogger? logger = null)
        {
            Rate = rate;
            Logger = logger ?? NullLogger.Instance;
            Ring = new long[rate];
        }

        internal void Limit(Action action)
        {
            lock (Ring)
            {
                if (Rate > 0)
                {
                    double wait = 1d + (Ring[Index] - Stopwatch.GetTimestamp()) / (double)Stopwatch.Frequency;
                    if (wait > 0)
                    {
                        Logger.LogTrace("{0} delay: {1:N4}", Index, wait);
                        Thread.Sleep(Convert.ToInt32(wait * 1000));
                    }
                    Ring[Index] = Stopwatch.GetTimestamp();
                    Index = (Index == Rate - 1) ? 0 : Index + 1;
                }
                action();
            }
        }
    }
}

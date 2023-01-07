using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System.Diagnostics;
using System.Threading;

namespace InterReact;

public sealed class RingLimiter
{
    private readonly object Locker = new();
    private readonly int Rate;
    private readonly ILogger Logger;
    private readonly long[] Ring;
    private int Index;

    internal RingLimiter(int rate = 0) : this(NullLogger.Instance, rate) { }
    internal RingLimiter(ILogger logger, int rate = 0)
    {
        Logger = logger ?? NullLogger.Instance;
        if (rate < 0)
            throw new ArgumentOutOfRangeException(nameof(rate));
        Rate = rate;
        Ring = new long[rate];
    }

    internal void Limit(Action action)
    {
        lock (Locker)
        {
            if (Rate > 0)
            {
                double wait = 1d + (Ring[Index] - Stopwatch.GetTimestamp()) / (double)Stopwatch.Frequency;
                if (wait > 0)
                {
                    Logger.LogTrace("{Index} delay: {Wait:N4}", Index, wait);
                    Thread.Sleep(Convert.ToInt32(wait * 1000));
                }
                Ring[Index] = Stopwatch.GetTimestamp();
                Index = (Index == Rate - 1) ? 0 : Index + 1;
            }
            action();
        }
    }
}

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Diagnostics;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Disposables;
using NodaTime;
using NodaTime.Text;

namespace InterReact;

public record struct ContractDetailsTimeEvent(ZonedDateTime Time, ContractTimeStatus Status);
public record struct ContractDetailsTimePeriod(ContractDetailsTimeEvent? Previous, ContractDetailsTimeEvent? Next);

public sealed class ContractDetailsTime
{
    private static readonly LocalTimePattern TimePattern = LocalTimePattern.CreateWithInvariantCulture("HHmm");
    private static readonly LocalDatePattern DatePattern = LocalDatePattern.CreateWithInvariantCulture("yyyyMMdd");
    private readonly ContractDetails ContractDetails;
    private readonly IScheduler TheScheduler;
    public DateTimeZone TimeZone { get; } // null if not available
    public IList<ContractDetailsTimeEvent> Events { get; } = new List<ContractDetailsTimeEvent>();
    // empty if no hours or timeZone
    public IObservable<ContractDetailsTimePeriod> ContractTimeObservable { get; } // completes immediately if no timeZone or hours

    public ContractDetailsTime(ContractDetails contractDetails, IScheduler? scheduler = null)
    {
        ArgumentNullException.ThrowIfNull(contractDetails);
        ContractDetails = contractDetails;
        TheScheduler = scheduler ?? Scheduler.Default;
        ContractTimeObservable = CreateContractTimeObservable();

        string tzi = contractDetails.TimeZoneId;
        if (string.IsNullOrEmpty(tzi)) // for expired Future Options there is no TimeZoneId(?)
            tzi = "Etc/GMT";
        TimeZone = DateTimeZoneProviders.Tzdb.GetZoneOrNull(tzi) ?? throw new ArgumentException($"TimeZoneId '{tzi}' not found.");
        Events = GetList();
    }

    private List<ContractDetailsTimeEvent> GetList()
    {
        // sorted list is used to ensure that dates are unique and to maintain order
        SortedList<LocalDateTime, ContractTimeStatus> list = new();

        foreach ((LocalDateTime start, LocalDateTime end) in GetSessions(ContractDetails.TradingHours))
        {
            list.Add(start, ContractTimeStatus.Trading);
            list.Add(end, ContractTimeStatus.Closed);
        }
        foreach ((LocalDateTime start, LocalDateTime end) in GetSessions(ContractDetails.LiquidHours))
        {
            KeyValuePair<LocalDateTime, ContractTimeStatus> previous = list.Where(x => x.Key < end).LastOrDefault();
            list.Add(start, ContractTimeStatus.Liquid);
            list.Add(end, previous.Value == ContractTimeStatus.Trading ? ContractTimeStatus.Trading : ContractTimeStatus.Closed);
        }
        return list.Select(x => new ContractDetailsTimeEvent(x.Key.InZoneLeniently(TimeZone), x.Value)).ToList();
    }

    private static List<(LocalDateTime start, LocalDateTime end)> GetSessions(string s)
    {
        List<(LocalDateTime start, LocalDateTime end)> list = new();

        if (string.IsNullOrEmpty(s))
            return list;

        string[] days = s.Split(';');
        foreach (string day in days)
        {
            string[] parts = day.Split(':');

            Debug.Assert(parts.Length == 2);
            LocalDate date = DatePattern.Parse(parts[0]).Value;
            IEnumerable<string> sessions = parts[1].Split(',').Distinct(); // the sessions may be repeated(?)
            foreach (string session in sessions.Where(x => x != "CLOSED"))
            {
                (LocalTime startTime, LocalTime endTime) = GetSessionTime(session);
                LocalDateTime start = date.At(startTime);
                LocalDateTime end = date.At(endTime);
                if (end < start)
                    start = start.PlusDays(-1); //end = end.PlusDays(1); ??
                if (list.Any() && (start <= list.Last().end || end <= start)) // ensure consecutive, non-overlapping periods
                    throw new InvalidDataException("Invalid period.");
                list.Add((start, end));
            }
        }
        return list;

        // local
        static (LocalTime start, LocalTime end) GetSessionTime(string session)
        {
            LocalTime[] times = session.Split('-').Select(t => TimePattern.Parse(t).Value).ToArray();
            Trace.Assert(times.Length == 2,"times.Length == 2");
            return (times[0], times[1]);
        }
    }

    // get the current time from the scheduler
    public ContractDetailsTimePeriod? Get() => Get(Instant.FromDateTimeOffset(TheScheduler.Now));
    public ContractDetailsTimePeriod? Get(Instant dt)
    {
        if (!Events.Any())
            return null;

        return new ContractDetailsTimePeriod(
            Events.LastOrDefault(x => x.Time.ToInstant() <= dt),
            Events.FirstOrDefault(x => x.Time.ToInstant() > dt));
    }

    /// <summary>
    /// Creates an observable which emits Contract time events for the specified contract details.
    /// </summary>
    private IObservable<ContractDetailsTimePeriod> CreateContractTimeObservable()
    {
        return Observable.Create<ContractDetailsTimePeriod>(observer =>
        {
            ContractDetailsTimePeriod? initialResult = Get();
            if (initialResult is not null)
                observer.OnNext(initialResult.Value);
            if (initialResult?.Next is null)
            {
                observer.OnCompleted();
                return Disposable.Empty;
            }

            void work(Action<DateTimeOffset> self)
            {
                try
                {
                    ContractDetailsTimePeriod? result = Get();
                    if (result is not null)
                        observer.OnNext(result.Value);
                    if (result?.Next is null)
                        observer.OnCompleted();
                    else
                        self(result.Value.Next.Value.Time.ToDateTimeOffset());
                }
#pragma warning disable CA1031
                catch (Exception e)
#pragma warning restore CA1031
                {
                    observer.OnError(e);
                }
            }

            return TheScheduler.Schedule(initialResult.Value.Next.Value.Time.ToDateTimeOffset(), work);
        });
    }
}

using NodaTime.Text;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
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
    // empty if no hours or timeZone
    public IReadOnlyList<ContractDetailsTimeEvent> Events { get; }
    // completes immediately if no timeZone or hours
    public IObservable<ContractDetailsTimePeriod> ContractTimeObservable { get; }

    public ContractDetailsTime(ContractDetails contractDetails, IScheduler? scheduler = null)
    {
        ArgumentNullException.ThrowIfNull(contractDetails);
        ContractDetails = contractDetails;
        TheScheduler = scheduler ?? Scheduler.Default;
        ContractTimeObservable = CreateContractTimeObservable();

        string tzi = contractDetails.TimeZoneId;
        // for expired Future Options there is no TimeZoneId(?)
        if (string.IsNullOrEmpty(tzi))
            tzi = "Etc/GMT";
        TimeZone = DateTimeZoneProviders.Tzdb.GetZoneOrNull(tzi) ?? throw new ArgumentException($"TimeZoneId '{tzi}' not found.");
        Events = GetList();
    }

    private List<ContractDetailsTimeEvent> GetList()
    {
        // sorted list is used to ensure that dates are unique and to maintain order
        SortedList<LocalDateTime, ContractTimeStatus> list = [];

        foreach ((LocalDateTime start, LocalDateTime end) in GetSessions(ContractDetails.TradingHours))
        {
            list.Add(start, ContractTimeStatus.Trading);
            list.Add(end, ContractTimeStatus.Closed);
        }
        foreach ((LocalDateTime start, LocalDateTime end) in GetSessions(ContractDetails.LiquidHours))
        {
            KeyValuePair<LocalDateTime, ContractTimeStatus> previous = list.LastOrDefault(x => x.Key < end);
            list.Add(start, ContractTimeStatus.Liquid);
            list.Add(end, previous.Value == ContractTimeStatus.Trading ? ContractTimeStatus.Trading : ContractTimeStatus.Closed);
        }
        return list.Select(x => new ContractDetailsTimeEvent(x.Key.InZoneLeniently(TimeZone), x.Value)).ToList();
    }

    private static List<(LocalDateTime start, LocalDateTime end)> GetSessions(string s)
    {
        List<(LocalDateTime start, LocalDateTime end)> list = [];

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
                if (list.Count != 0 && (start <= list.Last().end || end <= start)) // ensure consecutive, non-overlapping periods
                    throw new InvalidDataException("Invalid period.");
                list.Add((start, end));
            }
        }
        return list;

        // local
        static (LocalTime start, LocalTime end) GetSessionTime(string session)
        {
            LocalTime[] times = session.Split('-').Select(t => TimePattern.Parse(t).Value).ToArray();
            Trace.Assert(times.Length == 2, "times.Length == 2");
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
    [SuppressMessage("Usage", "CA1031", Scope = "member")]
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

            return TheScheduler.Schedule(initialResult.Value.Next.Value.Time.ToDateTimeOffset(), Work);

            void Work(Action<DateTimeOffset> self)
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
                catch (Exception e)
                {
                    observer.OnError(e);
                }
            }
        });
    }
}

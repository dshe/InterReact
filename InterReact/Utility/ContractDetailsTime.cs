using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Diagnostics;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Disposables;
using Microsoft.Extensions.Logging;
using NodaTime;
using NodaTime.Text;

namespace InterReact
{
    public sealed record ContractDataTimeEvent
    {
        public ZonedDateTime Time { get; init; }
        public ContractTimeStatus Status { get; init; }
    }

    public sealed record ContractDataTimePeriod
    {
        public ContractDataTimeEvent? Previous { get; init; }
        public ContractDataTimeEvent? Next { get; init; }
    }

    public sealed class ContractDataTime
    {
        private static readonly LocalTimePattern TimePattern = LocalTimePattern.CreateWithInvariantCulture("HHmm");
        private static readonly LocalDatePattern DatePattern = LocalDatePattern.CreateWithInvariantCulture("yyyyMMdd");
        private readonly ContractDetails ContractData;
        private readonly IScheduler Sched;
        public DateTimeZone TimeZone { get; } // null if not available
        public IReadOnlyList<ContractDataTimeEvent> Events { get; } = new List<ContractDataTimeEvent>();
        // empty if no hours or timeZone
        public IObservable<ContractDataTimePeriod> ContractTimeObservable { get; } // completes immediately if no timeZone or hours

        public ContractDataTime(ContractDetails contractData, ILogger logger, IScheduler? scheduler = null)
        {
            ContractData = contractData;
            Sched = scheduler ?? Scheduler.CurrentThread;
            ContractTimeObservable = CreateContractTimeObservable();

            var tzi = contractData.TimeZoneId;
            if (string.IsNullOrEmpty(tzi)) // for expired Future Options there is no TimeZoneId(?)
                tzi = "Etc/GMT";
            TimeZone = DateTimeZoneProviders.Tzdb.GetZoneOrNull(tzi) ?? throw new ArgumentException($"TimeZoneId '{tzi}' not found.");
            Events = GetList();
        }

        private List<ContractDataTimeEvent> GetList()
        {
            // sorted list is used to ensure that dates are unique and to maintain order
            var list = new SortedList<LocalDateTime, ContractTimeStatus>();

            foreach (var (start, end) in GetSessions(ContractData.TradingHours))
            {
                list.Add(start, ContractTimeStatus.Trading);
                list.Add(end, ContractTimeStatus.Closed);
            }
            foreach (var (start, end) in GetSessions(ContractData.LiquidHours))
            {
                var previous = list.Where(x => x.Key < end).LastOrDefault();
                list.Add(start, ContractTimeStatus.Liquid);
                list.Add(end, previous.Value == ContractTimeStatus.Trading ? ContractTimeStatus.Trading : ContractTimeStatus.Closed);
            }
            return list.Select(x => new ContractDataTimeEvent { Time = x.Key.InZoneLeniently(TimeZone), Status = x.Value }).ToList();
        }

        private static List<(LocalDateTime start, LocalDateTime end)> GetSessions(string s)
        {
            var list = new List<(LocalDateTime start, LocalDateTime end)>();

            if (string.IsNullOrEmpty(s))
                return list;

            var days = s.Split(';');
            foreach (var day in days)
            {
                var parts = day.Split(':');

                Debug.Assert(parts.Count() == 2);
                var date = DatePattern.Parse(parts[0]).Value;
                var sessions = parts[1].Split(',').Distinct(); // the sessions may be repeated(?)
                foreach (var session in sessions.Where(x => x != "CLOSED"))
                {
                    var times = GetSessionTime(session);
                    var start = date.At(times.start);
                    var end = date.At(times.end);
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
                var times = session.Split('-').Select(t => TimePattern.Parse(t).Value).ToList();
                Debug.Assert(times.Count == 2);
                return (times[0], times[1]);
            }
        }

        // get the current time from the scheduler
        public ContractDataTimePeriod? Get() => Get(Instant.FromDateTimeOffset(Sched.Now));

        public ContractDataTimePeriod? Get(Instant dt)
        {
            if (!Events.Any())
                return null;

            return new ContractDataTimePeriod
            {
                Previous = Events.Where(x => x.Time.ToInstant() <= dt).LastOrDefault(),
                Next = Events.Where(x => x.Time.ToInstant() > dt).FirstOrDefault()
            };
        }

        /// <summary>
        /// Creates an observable which emits Contract time events for the specified contract details.
        /// </summary>
        private IObservable<ContractDataTimePeriod> CreateContractTimeObservable()
        {
            return Observable.Create<ContractDataTimePeriod>(observer =>
            {
                var initialResult = Get();
                if (initialResult != null)
                    observer.OnNext(initialResult);
                if (initialResult?.Next == null)
                {
                    observer.OnCompleted();
                    return Disposable.Empty;
                }

                void work(Action<DateTimeOffset> self)
                {
                    try
                    {
                        var result = Get();
                        if (result != null)
                            observer.OnNext(result);
                        if (result?.Next == null)
                            observer.OnCompleted();
                        else
                            self(result.Next.Time.ToDateTimeOffset());
                    }
                    catch (Exception e)
                    {
                        observer.OnError(e);
                    }
                }

                return Sched.Schedule(initialResult.Next.Time.ToDateTimeOffset(), work);
            });
        }
    }
}

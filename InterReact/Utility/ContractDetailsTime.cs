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
    public sealed record ContractDataTimeEvent(ZonedDateTime Time, ContractTimeStatus Status);
    public sealed record ContractDataTimePeriod(ContractDataTimeEvent? Previous, ContractDataTimeEvent? Next);

    public sealed class ContractDataTime
    {
        private static readonly LocalTimePattern TimePattern = LocalTimePattern.CreateWithInvariantCulture("HHmm");
        private static readonly LocalDatePattern DatePattern = LocalDatePattern.CreateWithInvariantCulture("yyyyMMdd");
        private readonly ContractDetails ContractData;
        private readonly IScheduler TheScheduler;
        public DateTimeZone TimeZone { get; } // null if not available
        public IReadOnlyList<ContractDataTimeEvent> Events { get; } = new List<ContractDataTimeEvent>();
        // empty if no hours or timeZone
        public IObservable<ContractDataTimePeriod> ContractTimeObservable { get; } // completes immediately if no timeZone or hours

        public ContractDataTime(ContractDetails contractData, ILogger logger, IScheduler? scheduler = null)
        {
            ContractData = contractData;
            TheScheduler = scheduler ?? Scheduler.Default;
            ContractTimeObservable = CreateContractTimeObservable();

            string tzi = contractData.TimeZoneId;
            if (string.IsNullOrEmpty(tzi)) // for expired Future Options there is no TimeZoneId(?)
                tzi = "Etc/GMT";
            TimeZone = DateTimeZoneProviders.Tzdb.GetZoneOrNull(tzi) ?? throw new ArgumentException($"TimeZoneId '{tzi}' not found.");
            Events = GetList();
        }

        private List<ContractDataTimeEvent> GetList()
        {
            // sorted list is used to ensure that dates are unique and to maintain order
            SortedList<LocalDateTime, ContractTimeStatus> list = new();

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
            //return list.Select(x => new ContractDataTimeEvent { Time = x.Key.InZoneLeniently(TimeZone), Status = x.Value }).ToList();
            return list.Select(x => new ContractDataTimeEvent(x.Key.InZoneLeniently(TimeZone), x.Value)).ToList();
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
                var sessions = parts[1].Split(',').Distinct(); // the sessions may be repeated(?)
                foreach (string session in sessions.Where(x => x != "CLOSED"))
                {
                    var times = GetSessionTime(session);
                    LocalDateTime start = date.At(times.start);
                    LocalDateTime end = date.At(times.end);
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
                Debug.Assert(times.Length == 2);
                return (times[0], times[1]);
            }
        }

        // get the current time from the scheduler
        public ContractDataTimePeriod? Get() => Get(Instant.FromDateTimeOffset(TheScheduler.Now));

        public ContractDataTimePeriod? Get(Instant dt)
        {
            if (!Events.Any())
                return null;

            return new ContractDataTimePeriod(
                Events.LastOrDefault(x => x.Time.ToInstant() <= dt),
                Events.FirstOrDefault(x => x.Time.ToInstant() > dt));
        }

        /// <summary>
        /// Creates an observable which emits Contract time events for the specified contract details.
        /// </summary>
        private IObservable<ContractDataTimePeriod> CreateContractTimeObservable()
        {
            return Observable.Create<ContractDataTimePeriod>(observer =>
            {
                ContractDataTimePeriod? initialResult = Get();
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
                        ContractDataTimePeriod? result = Get();
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

                return TheScheduler.Schedule(initialResult.Next.Time.ToDateTimeOffset(), work);
            });
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using InterReact.Enums;
using InterReact.Messages;
using NodaTime;
using System.Diagnostics;
using NodaTime.Text;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Disposables;

namespace InterReact.Utility
{
    // use class/reference type to allow use of null to indicate no data
    public sealed class ContractDetailsTimeEvent 
    {
        public ZonedDateTime      Time   { get; internal set; }
        public ContractTimeStatus Status { get; internal set; }
    }

    public sealed class ContractDetailsTimePeriod
    {
        public ContractDetailsTimeEvent Previous { get; internal set; }
        public ContractDetailsTimeEvent Next     { get; internal set; }
    }

    public sealed class ContractDetailsTime
    {
        private static readonly LocalTimePattern TimePattern = LocalTimePattern.CreateWithInvariantCulture("HHmm");
        private static readonly LocalDatePattern DatePattern = LocalDatePattern.CreateWithInvariantCulture("yyyyMMdd");
        private readonly ContractDetails ContractDetails;
        private readonly IScheduler Sched;
        public DateTimeZone TimeZone { get; } // null if not available
        public IReadOnlyList<ContractDetailsTimeEvent> Events { get; } // empty if no hours or timeZone
        public IObservable<ContractDetailsTimePeriod> ContractTimeObservable { get; } // completes immediately if no timeZone or hours

        public ContractDetailsTime(ContractDetails contractDetails, IScheduler scheduler = null)
        {
            ContractDetails = contractDetails ?? throw new ArgumentNullException(nameof(contractDetails));
            Sched = scheduler ?? Scheduler.CurrentThread; // shared EventLoopScheduler?
            ContractTimeObservable = CreateContractTimeObservable();
            var tzi = contractDetails.TimeZoneId;
            if (string.IsNullOrEmpty(tzi)) // for expired Future Options there is no TimeZoneId(?)
                return;
            TimeZone = DateTimeZoneProviders.Tzdb.GetZoneOrNull(tzi) ?? throw new ArgumentException($"TimeZoneId '{tzi}' not found.");
            Events = GetList();
        }

        private List<ContractDetailsTimeEvent> GetList()
        {
            // sorted list is used to ensure that dates are unique and to maintain order
            var list = new SortedList<LocalDateTime, ContractTimeStatus>();

            foreach (var session in GetSessions(ContractDetails.TradingHours))
            {
                list.Add(session.start, ContractTimeStatus.Trading);
                list.Add(session.end,   ContractTimeStatus.Closed);
            }
            foreach (var session in GetSessions(ContractDetails.LiquidHours))
            {
                var previous = list.Where(x => x.Key < session.end).LastOrDefault();
                list.Add(session.start, ContractTimeStatus.Liquid);
                list.Add(session.end, previous.Value == ContractTimeStatus.Trading ? ContractTimeStatus.Trading: ContractTimeStatus.Closed);
            }
            return list.Select(x => new ContractDetailsTimeEvent { Time = x.Key.InZoneLeniently(TimeZone), Status = x.Value }).ToList();
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
                    var end   = date.At(times.end);
                    if (end < start)
                        start = start.PlusDays(-1); //end = end.PlusDays(1); ??
                    if (list.Any() && (start <= list.Last().end || end <= start)) // ensure consecutive, non-overlapping periods
                        throw new InvalidDataException("Invalid period.");
                    list.Add((start, end));
                }
            }
            return list;

            // local
            (LocalTime start, LocalTime end) GetSessionTime(string session)
            {
                var times = session.Split('-').Select(t => TimePattern.Parse(t).Value).ToList();
                Debug.Assert(times.Count == 2);
                return (times[0], times[1]);
            }
        }

        // get the current time from the scheduler
        public ContractDetailsTimePeriod Get() => Get(Instant.FromDateTimeOffset(Sched.Now));

        public ContractDetailsTimePeriod Get(Instant dt)
        {
            if (Events == null)
                return null;

            return new ContractDetailsTimePeriod
            {
                Previous = Events.Where(x => x.Time.ToInstant() <= dt).LastOrDefault(),
                Next     = Events.Where(x => x.Time.ToInstant() >  dt).FirstOrDefault()
            };
        }

        /// <summary>
        /// Creates an observable which emits Contract time events for the specified contract details.
        /// </summary>
        private IObservable<ContractDetailsTimePeriod> CreateContractTimeObservable()
        {
            return Observable.Create<ContractDetailsTimePeriod>(observer =>
            {
                var initialResult = Get();
                if (initialResult != null)
                    observer.OnNext(initialResult);
                if (initialResult?.Next == null)
                {
                    observer.OnCompleted();
                    return Disposable.Empty;
                }

                Action<Action<DateTimeOffset>> work = (self) =>
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
                };

                return Sched.Schedule(initialResult.Next.Time.ToDateTimeOffset(), work);
            });
        }
    }
}

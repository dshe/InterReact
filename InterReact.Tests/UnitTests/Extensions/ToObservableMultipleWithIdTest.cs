﻿using System;
using System.Diagnostics;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace InterReact.UnitTests.Extensions
{
    public class ToObservableMultipleWithIdTest : UnitTestsBase
    {
        private const int Id = 42;
        private int subscribeCalls;
        private readonly Subject<object> subject = new();

        public interface ISomeClass : IHasRequestId { }
        public class SomeClass : ISomeClass
        {
            public int RequestId { get; } = Id;
            public long Ticks { get; } = Stopwatch.GetTimestamp();
        }
        public class SomeClassEnd : ISomeClass
        {
            public int RequestId { get; } = Id;
        }

        public ToObservableMultipleWithIdTest(ITestOutputHelper output) : base(output) { }


        [Fact]
        public async Task Test_Multi_Ok()
        {
            var observable = subject.ToObservableMultipleWithId<SomeClassEnd>(
                () => Id,
                requestId =>
                {
                    Assert.Equal(Id, requestId);
                    Interlocked.Increment(ref subscribeCalls);
                    subject.OnNext(new SomeClass());
                    subject.OnNext(new SomeClass());
                    subject.OnNext(new SomeClassEnd());
                });
            var list = await observable.ToList();
            Assert.Equal(2, list.Count);
            Assert.Equal(1, subscribeCalls);
        }

        [Fact]
        public async Task Test_Alert_multi()
        {
            var observable = subject.ToObservableMultipleWithId<SomeClassEnd>(
                () => Id,
                requestId =>
                {
                    Assert.Equal(Id, requestId);
                    Interlocked.Increment(ref subscribeCalls);
                    subject.OnNext(new Alert(requestId, 1, "some error"));
                });
            var alert = await observable;
            Assert.IsType<Alert>(alert);
            Assert.Equal(Id, alert.RequestId);
            Assert.Equal(1, subscribeCalls);
        }

        [Fact]
        public void Test_Unsubscribe_Error()
        {
            var observable = subject.ToObservableSingleWithId(
                () => Id,
                requestId => Interlocked.Increment(ref subscribeCalls),
                requestId => { throw new BarrierPostPhaseException(); });

            Assert.Throws<BarrierPostPhaseException>(() => observable.Subscribe().Dispose());
            Assert.Equal(1, subscribeCalls);
        }
    }

}
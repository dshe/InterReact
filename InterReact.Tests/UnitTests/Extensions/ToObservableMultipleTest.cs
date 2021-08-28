using System;
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
    public class ToObservableMultipleTest : UnitTestsBase
    {
        public ToObservableMultipleTest(ITestOutputHelper output) : base(output) { }

        private int subscribeCalls;
        private readonly Subject<object> subject = new();

        public class SomeClass
        {
            public long Time { get; } = Stopwatch.GetTimestamp();
        }

        public class SomeClassEnd { }


        [Fact]
        public async Task Test_Multi_Ok()
        {
            var observable = subject.ToObservableMultiple<object, SomeClassEnd>(
                () =>
                {
                    Interlocked.Increment(ref subscribeCalls);
                    subject.OnNext(new SomeClass());
                    subject.OnNext(new SomeClass());
                    subject.OnNext(new SomeClassEnd());
                });
            var list = await observable.ToList();
            Assert.Equal(2, list.Count);
            Assert.Equal(1, subscribeCalls);
        }

    }

}

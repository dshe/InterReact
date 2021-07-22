using InterReact.SystemTests;
using Stringification;
using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace InterReact.SystemTests.Other
{
    public class NewsBulletinTests : TestCollectionBase
    {
        public NewsBulletinTests(ITestOutputHelper output, TestFixture fixture) : base(output, fixture) { }

        [Fact]
        public async Task TestNewsBulletins()
        {
            //var observable = Client.Services.CreateNewsBulletinsObservable();
            var observable = Client.Services.NewsBulletinsObservable;
            var sub = observable.Select(x => x.Stringify()).Subscribe(x => Write(x));
            // allow some time to print news bulletins, if any, to the console.
            await Task.Delay(1000);
            sub.Dispose();
        }

    }
}
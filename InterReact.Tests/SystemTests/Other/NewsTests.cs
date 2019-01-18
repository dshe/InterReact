using System;
using System.Threading.Tasks;
using Stringification;
using InterReact.Tests.Utility;
using Xunit;
using Xunit.Abstractions;
using InterReact.Extensions;

namespace InterReact.Tests.SystemTests.Other
{
    public class NewsBulletinTests : BaseSystemTest
    {
        public NewsBulletinTests(SystemTestFixture fixture, ITestOutputHelper output) : base(fixture, output) { }

        [Fact]
        public async Task TestNewsBulletins()
        {
            var sub = Client.Services.NewsBulletinsObservable.Stringify().Subscribe(Write);
            // allow some time to print news bulletins, if any, to the console.
            await Task.Delay(1000);
            sub.Dispose();
        }

    }
}
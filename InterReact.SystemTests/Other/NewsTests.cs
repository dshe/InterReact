using System;
using System.Threading.Tasks;
using Stringification;
using InterReact.SystemTests;
using Xunit;
using Xunit.Abstractions;

namespace SystemTests.Other
{
    public class NewsBulletinTests : BaseTest
    {
        public NewsBulletinTests(ITestOutputHelper output) : base(output) { }

        [Fact]
        public async Task TestNewsBulletins()
        {
            var sub = Client.Services.CreateNewsBulletinsObservable().StringifyItems().Subscribe(x => Write(x));
            // allow some time to print news bulletins, if any, to the console.
            await Task.Delay(1000);
            sub.Dispose();
        }

    }
}
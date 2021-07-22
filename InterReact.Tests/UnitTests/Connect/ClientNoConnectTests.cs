using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace InterReact.UnitTests.NoConnect
{
    public class Client_No_Connect_Tests : UnitTestsBase
    {
        public Client_No_Connect_Tests(ITestOutputHelper output) : base(output) { }

        [Fact]
        public async Task T01_Cancel()
        {
            var ct = new CancellationToken(true);
            var task = new InterReactClientBuilder(Logger).BuildAsync(ct);
            var ex = await Assert.ThrowsAnyAsync<OperationCanceledException>(async () => await task);
            Write(ex.ToString());
        }

        [Fact]
        public async Task T02_Timeout()
        {
            var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(1000));
            var task = new InterReactClientBuilder(Logger).BuildAsync(cts.Token);
            var ex = await Assert.ThrowsAsync<OperationCanceledException>(async () => await task);
            Write(ex.ToString());
        }

        [Fact]
        public async Task T03_Connection_Refused()
        {
            var task = new InterReactClientBuilder(Logger).SetPort(999).BuildAsync();
            var ex = await Assert.ThrowsAsync<ArgumentException>(async () => await task);
            Write(ex.ToString());
        }

    }
}

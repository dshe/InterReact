using System;
using System.Collections.Generic;
using System.IO;
using System.Reactive.Linq;
using System.Threading.Tasks;
using InterReact.Extensions;
using InterReact.Messages;
using InterReact.Tests.Utility;
using InterReact.Tests.Utility.AutoData;
using Xunit;
using Xunit.Abstractions;
using Microsoft.Extensions.Logging;

namespace InterReact.Tests.UnitTests.Extensions
{
    public class ContractDataSingleTest : BaseUnitTest
    {
        public ContractDataSingleTest(ITestOutputHelper output) : base(output) { }

        /*
        [Fact]
        public async Task Test_Null()
        {
            IObservable<IReadOnlyList<ContractData>> nullvalue;
            var ex = await Assert.ThrowsAsync<ArgumentNullException>
                    (async () => await nullvalue.ContractDataSingle());
            ex.WriteMessageTo(Write);
        }
        */

        [Fact]
        public async Task Test_Empty()
        {
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
                        await Observable.Empty<List<ContractData>>().ContractDataSingle());
            ex.WriteMessageTo(Logger);
            Logger.LogInformation("test");
        }

        [Fact]
        public async Task Test_One_Item()
        {
            var list = new List<ContractData> { new ContractData() };
            var listOfList = new List<List<ContractData>> {list};
            var cd = await listOfList.ToObservable().ContractDataSingle();
            Logger.LogDebug(cd.Stringify());
        }

        [Fact]
        public async Task Test_Multiple()
        {
            var list = new List<ContractData> { new ContractData(), new ContractData() };
            var listOfList = new List<List<ContractData>> {list};
            var cd = listOfList.ToObservable().ContractDataSingle();
            (await Assert.ThrowsAsync<InvalidDataException>(async () => await cd))
                .WriteMessageTo(Logger);
        }

    }
}

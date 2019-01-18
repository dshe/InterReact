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

namespace InterReact.Tests.UnitTests.Extensions
{
    public class ContractDetailsSingleTest : BaseUnitTest
    {
        public ContractDetailsSingleTest(ITestOutputHelper output) : base(output) { }

        [Fact]
        public async Task Test_Null()
        {
            IObservable<IReadOnlyList<ContractDetails>> nullvalue = null;
            var ex = await Assert.ThrowsAsync<ArgumentNullException>
                    (async () => await nullvalue.ContractDetailsSingle());
            ex.WriteMessageTo(Write);
        }

        [Fact]
        public async Task Test_Empty()
        {
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
                        await Observable.Empty<List<ContractDetails>>().ContractDetailsSingle());
            ex.WriteMessageTo(Write);
        }

        [Fact]
        public async Task Test_One_Item()
        {
            var list = AutoData.Create<ContractDetails>(1);
            var listOfList = new List<List<ContractDetails>> {list};
            var cd = await listOfList.ToObservable().ContractDetailsSingle();
            Write(cd.Stringify());
        }

        [Fact]
        public async Task Test_Multiple()
        {
            var list = AutoData.Create<ContractDetails>(2);
            var listOfList = new List<List<ContractDetails>> {list};
            var cd = listOfList.ToObservable().ContractDetailsSingle();
            var ex = await Assert.ThrowsAsync<InvalidDataException>(async () => await cd);
            ex.WriteMessageTo(Write);
        }

    }
}

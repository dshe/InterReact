using InterReact.Extensions;
using Stringification;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace InterReact.UnitTests.Extensions
{
    public class Contract_Data_Single_Test : BaseUnitTest
    {
        public Contract_Data_Single_Test(ITestOutputHelper output) : base(output) { }

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
                        await Observable.Empty<List<ContractDetails>>().ContractDataSingle());
            Write(ex.ToString());
        }

        [Fact]
        public async Task Test_One_Item()
        {
            var list = new List<ContractDetails> { new ContractDetails() };
            var listOfList = new List<List<ContractDetails>> { list };
            var cd = await listOfList.ToObservable().ContractDataSingle();
            Write(cd.Stringify());
        }

        [Fact]
        public async Task Test_Multiple()
        {
            var list = new List<ContractDetails> { new ContractDetails(), new ContractDetails() };
            var listOfList = new List<List<ContractDetails>> { list };
            var cd = listOfList.ToObservable().ContractDataSingle();
            var ex = await Assert.ThrowsAsync<InvalidDataException>(async () => await cd);
            Write(ex.ToString());
        }

    }
}

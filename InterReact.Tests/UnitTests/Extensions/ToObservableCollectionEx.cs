using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using Xunit;
using Xunit.Abstractions;

namespace InterReact.UnitTests.Extensions
{
    public class Item
    {
        public string Key1 { get; set; }
        public string Data1 { get; set; }
        public Item(string key1, string data1 = "")
        {
            Key1 = key1;
            Data1 = data1;
        }
    }


    public class ToObservableCollectionEx : BaseUnitTest
    {
        public ToObservableCollectionEx(ITestOutputHelper output) : base(output) { }

        /*
    [Theory]
    [InlineData("a")]
    [InlineData("a", "a")]
    [InlineData("a", "c", "b")]
    [InlineData("z", "", "b", "zzz", "b", "")]
    public void T01_Empty(params string[] data)
    {
        var list1 = data.Select(x => new Item(x)).ToList();

        // sequence without OnCompleted
        var observable = list1.ToObservable().Concat(Observable.Never<Item>());

        var oc = observable.ToObservableCollection(x => x.Key1);
        var list2 = new List<Item>(oc);

        var expected = list1.DistinctBy(a => a.Key1).OrderBy(x => x.Key1).ToList();

        Assert.Equal(expected.Count(), list2.Count);

        for (var i = 0; i < expected.Count(); i++)
        {
            Assert.Equal(expected[i].Key1, list2[i].Key1);
            Assert.Equal(expected[i].Data1, list2[i].Data1);
        }
    }
*/

        /*
        [Fact]
        public void T00_Empty_Strike()
        {
            var list = new[] { new Item("b"), new Item("a"), new Item("b", "x") };

            var observable = list.ToObservable().Concat(Observable.Never<Item>());

            // sequence without OnCompleted
            var oc = observable.ToObservableCollection(x => x.Key1);

            Assert.Equal(2, oc.Count);
            Assert.Equal("a", oc[0].Key1);
            Assert.Equal("", oc[0].Data1);
            Assert.Equal("b", oc[1].Key1);
            Assert.Equal("x", oc[1].Data1);
        }
        */

    }
}


using InterReact.Tests.Utility;
using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Text;
using Xunit;
using Xunit.Abstractions;
using InterReact.Extensions;
using System.ComponentModel;
using InterReact.Utility;
using System.Linq;
using System.Reactive.Disposables;

namespace InterReact.Tests.UnitTests.Extensions
{
    public class Item : NotifyPropertyChanged
    {
        public string Key1 { get; set; }
        public string Data1 { get; set; }
    }

    public class ToObservableCollectionEx : BaseUnitTest
    {
        public ToObservableCollectionEx(ITestOutputHelper output) : base(output) { }

        [Theory]
        [InlineData("a")]
        [InlineData("a", "a")]
        [InlineData("a", "c", "b")]
        [InlineData("z", "", "b", "zzz", "b", "")]
        public void T01_Empty(params string[] data)
        {
            var list1 = data.Select(x => new Item { Key1 = x }).ToList();

            var oc = list1.ToTestObservable(complete:false).ToObservableCollection(x => x.Key1);
            var list2 = new List<Item>(oc);

            var expected = list1.DistinctBy(a => a.Key1).OrderBy(x => x.Key1).ToList();

            Assert.Equal(expected.Count(), list2.Count);

            for (var i = 0; i < expected.Count(); i++)
            {
                Assert.Equal(expected[i].Key1, list2[i].Key1);
                Assert.Equal(expected[i].Data1, list2[i].Data1);
            }
        }

        [Fact]
        public void T00_Empty_Strike()
        {
            var list = new[] { new Item { Key1 = "b" }, new Item { Key1 = "a" }, new Item { Key1 = "b", Data1 = "x" } };

            var oc = list.ToTestObservable(complete:false).ToObservableCollection(x => x.Key1);

            Assert.Equal(2, oc.Count);
            Assert.Equal("a", oc[0].Key1);
            Assert.Null(oc[0].Data1);
            Assert.Equal("b", oc[1].Key1);
            Assert.Equal("x", oc[1].Data1);

            //var list2 = new List<Item>(oc);
            ;

        }

    }
}


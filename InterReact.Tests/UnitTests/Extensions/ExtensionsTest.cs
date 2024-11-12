using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
namespace Extension;

public sealed class ExtensionsTest(ITestOutputHelper output) : UnitTestBase(output)
{
    [Fact]
    public async Task AccumulateTest()
    {
        IObservable<object> observable = Observable.Create<object>(observer =>
        {
            observer.OnNext("1");
            observer.OnNext(2);
            observer.OnNext("1");
            observer.OnNext("5");
            observer.OnNext("2");
            observer.OnNext(5);
            observer.OnCompleted();
            return Disposable.Empty;
        });

        //IObservable<IList<IList<string>>> xx = observable.Accumulate<string, int>().ToList();
        IList<string[]> xx = await observable.Accumulate<string, int>().ToList();

        foreach (string[] x in xx)
        {
            Write("! ");
            foreach (string y in x)
            {
                Write(y.ToString());
            }
        }

    }

}

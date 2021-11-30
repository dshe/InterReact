using Microsoft.Extensions.Logging;
using System;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace InterReact.UnitTests.Experimental;

/* 
 * Errors must be observed.
 * Errors will be observed when awaiting an observable
 * or errors can be observed when subscribing to the OnError channel
 */
public class Observable_Error : UnitTestsBase
{
    public Observable_Error(ITestOutputHelper output) : base(output) { }

    private readonly IObservable<string> myObservable = Observable.Create<string>(observer =>
    {
        observer.OnNext("1");
        observer.OnError(new Exception("ex!"));
        return Disposable.Empty;
    });

    [Fact]
    public async Task ObservableOnError1()
    {
        await Assert.ThrowsAnyAsync<Exception>(async () => await myObservable);

        Assert.ThrowsAny<Exception>(() => myObservable.Subscribe(x => Logger.LogInformation(x)));

        // exception is observed, so not thrown
        myObservable.Subscribe(
            x => Logger.LogInformation(x),
            e => Logger.LogInformation(e.Message),
            () => Logger.LogInformation("OnComplete?"));

        // no problem
        await Task.Delay(500);
    }

    [Fact]
    public async Task ObservableOnError2()
    {
        var observable = myObservable.ObserveOn(TaskPoolScheduler.Default);

        // same
        await Assert.ThrowsAnyAsync<Exception>(async () => await observable);

        // DOES NOT THROW!
        observable.Subscribe(x => Logger.LogInformation(x));

        // same, exception is observed
        observable.Subscribe(
            x => Logger.LogInformation(x),
            e => Logger.LogInformation(e.Message),
            () => Logger.LogInformation("OnComplete?"));

        await Task.Delay(500);
    }
}

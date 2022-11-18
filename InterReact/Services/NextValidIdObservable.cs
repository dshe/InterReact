using System.Reactive.Linq;

namespace InterReact;

public partial class Service
{
    /// <summary>
    /// An observable which emits the current time, then completes.
    /// </summary>
    public IObservable<int> NextValidIdObservable { get; }

    private IObservable<int> CreateNextValidIdObservable() =>
        Response
            .ToObservableSingle<NextId>(() => Request.RequestIds(-1))
            .Select(m => m.Id)
            .ShareSource();
}
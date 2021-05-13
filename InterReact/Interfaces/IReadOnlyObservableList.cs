using System.Collections.Generic;
using System.Collections.Specialized;

namespace InterReact
{
    public interface IReadOnlyObservableList<out T> : IReadOnlyList<T>, INotifyCollectionChanged
    {
    }
}

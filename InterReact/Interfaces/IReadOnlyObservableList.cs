using System.Collections.Generic;
using System.Collections.Specialized;

namespace InterReact.Interfaces
{
    public interface IReadOnlyObservableList<out T> : IReadOnlyList<T>, INotifyCollectionChanged
    {
    }
}

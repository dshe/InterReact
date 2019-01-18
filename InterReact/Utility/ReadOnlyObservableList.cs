using InterReact.Interfaces;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

// https://connect.microsoft.com/VisualStudio/feedback/details/641395/readonlyobservablecollection-t-collectionchanged-event-should-be-public

namespace InterReact.Utility
{
    public class ReadOnlyObservableList<T> : ReadOnlyObservableCollection<T>, IReadOnlyObservableList<T>
    {
        public ReadOnlyObservableList(ObservableCollection<T> list) : base(list)
        {
        }

        public new event NotifyCollectionChangedEventHandler CollectionChanged
        {
            add { base.CollectionChanged += value; }
            remove { base.CollectionChanged -= value; }
        }

        public new event PropertyChangedEventHandler PropertyChanged
        {
            add { base.PropertyChanged += value; }
            remove { base.PropertyChanged -= value; }
        }
    }
}

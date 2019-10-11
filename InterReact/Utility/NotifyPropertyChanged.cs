using System.ComponentModel;

#nullable enable

namespace InterReact.Utility
{
    public abstract class NotifyPropertyChanged : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        internal void PropertyChange(string? propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

}

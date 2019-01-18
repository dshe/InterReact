using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;

namespace UwpRealtimeVolume.Utility
{
    public class BindableObserver<T> : IObserver<T>, INotifyPropertyChanged
    {
        public string Text
        {
            get
            {
                if (Equals(value, default))
                    return string.Empty;
                return string.Format(CultureInfo.CurrentUICulture, Format, value);
            }
        }

        private string Format { get; }

        public BindableObserver(string format = "{0}")
        {
            Format = format;
        }

        private T value;
        public T Value
        {
            get => value;
            set
            {
                if (EqualityComparer<T>.Default.Equals(this.value, value))
                    return;
                this.value = value;
                NotifyAsync();
            }
        }

        //////////////////////////////////////////////////////

        private async void NotifyAsync()
        {
            var dispatcher = CoreApplication.MainView.CoreWindow.Dispatcher;
            if (dispatcher.HasThreadAccess)
                Notify();
            else
                await dispatcher.RunAsync(CoreDispatcherPriority.Normal, Notify);
        }

        private void Notify()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Value)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Text)));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        //////////////////////////////////////////////////////

        public void OnNext(T v) => Value = v;
        public void OnError(Exception error) {}
        public void OnCompleted() {}


    }
}

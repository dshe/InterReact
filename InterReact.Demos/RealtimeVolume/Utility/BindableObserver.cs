using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;

#nullable enable

namespace RealtimeVolume.Utility
{
    public class BindableObserver<T> : IObserver<T>, INotifyPropertyChanged
    {
        private readonly string Format;
        private string text = "";
        public string Text
        {
            get => text;
            set
            {
                if (value == text)
                    return;
                text = value;
                NotifyAsync();
            }
        }

        public BindableObserver(T initialValue, string format = "{0}")
        {
            value = initialValue;
            Format = format;
            text = string.Format(CultureInfo.InvariantCulture, Format, value);
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
                Text = string.Format(CultureInfo.InvariantCulture, Format, value);
            }
        }

        public void OnNext(T v) => Value = v;
        public void OnError(Exception error) { }
        public void OnCompleted() { }

        //////////////////////////////////////////////////////

        private async void NotifyAsync()
        {
            var dispatcher = CoreApplication.MainView.CoreWindow.Dispatcher;
            if (dispatcher.HasThreadAccess)
                Notify();
            else
                await dispatcher.RunAsync(CoreDispatcherPriority.Normal, Notify);
        }

        private void Notify() => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Text)));

        public event PropertyChangedEventHandler PropertyChanged;
    }


}

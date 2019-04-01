using System;
using System.ComponentModel;
using System.Reactive;
using System.Reactive.Subjects;
using Windows.UI.Xaml.Controls;

#nullable enable

namespace RealtimeVolume.Utility
{
    public class TextBoxChangingObservable : ObservableBase<string>, INotifyPropertyChanged, IDisposable
    {
        private readonly Subject<string> subject = new Subject<string>();
        protected override IDisposable SubscribeCore(IObserver<string> observer) => subject.Subscribe(observer);

        public void TextChanged(object sender, TextChangedEventArgs e)
        {
            //var start = textBox.SelectionStart;
            //textBox.Text = textBox.Text.ToUpper();
            //textBox.SelectionStart = start;
            //subject.OnNext(textBox.Text);
        }

        public void TextChanging(TextBox textBox, TextBoxTextChangingEventArgs e)
        {
            var start = textBox.SelectionStart;
            textBox.Text = textBox.Text.ToUpper();
            textBox.SelectionStart = start;
            subject.OnNext(textBox.Text);
        }

        private bool isEnabled = true;
        public bool IsEnabled 
        {
            get => isEnabled;
            set
            {
                if (isEnabled == value)
                    return;
                isEnabled = value;
                Notify();
            }
        }

        private void Notify() => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsEnabled)));

        public event PropertyChangedEventHandler PropertyChanged;

        public void Dispose() => subject.Dispose();
    }
}

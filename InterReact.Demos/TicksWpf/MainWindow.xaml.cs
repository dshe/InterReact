using InterReact;
using Stringification;
using System;
using System.Linq;
using System.ComponentModel;
using System.Diagnostics;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Windows.Media;
using System.Threading.Tasks;
using System.Threading;

namespace TicksWpf
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public string Symbol
        {
            set => SymbolSubject.OnNext(value);
        }
        private readonly Subject<string> SymbolSubject = new();

        private string description = "";
        public string Description
        {
            get => description;
            private set
            {
                if (value == description)
                    return;
                description = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Description)));
            }
        }

        private double bidPrice;
        public double BidPrice
        {
            get => bidPrice;
            private set
            {
                if (value == bidPrice)
                    return;
                bidPrice = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(BidPrice)));
            }
        }

        private double askPrice;
        public double AskPrice
        {
            get => askPrice;
            private set
            {
                if (value == askPrice)
                    return;
                askPrice = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AskPrice)));
            }
        }

        private double lastPrice;
        public double LastPrice
        {
            get => lastPrice;
            private set
            {
                if (value == lastPrice)
                    return;
                lastPrice = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LastPrice)));
            }
        }

        private double changePrice;
        public double ChangePrice
        {
            get => changePrice;
            private set
            {
                if (value == changePrice)
                    return;
                changePrice = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ChangePrice)));
            }
        }

        private SolidColorBrush changeColor = Brushes.Transparent;
        public SolidColorBrush ChangeColor
        {
            get => changeColor;
            private set
            {
                if (value == changeColor)
                    return;
                changeColor = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ChangeColor)));
            }
        }

        ////////////////////////////////////////////////////////////////////

        private IInterReactClient? Client;
        private IDisposable TicksConnection = Disposable.Empty;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
        }

        private async void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            try
            {
                Client = await InterReactClientBuilder.Create().BuildAsync();
            }
            catch (Exception exception)
            {
                Utilities.MessageBoxShow(exception.Message, "InterReact", terminate: true);
                return;
            }

            Client.Response.Subscribe(x => Debug.WriteLine(x.Stringify()));

            int requestId = Client.Request.GetNextId();

            Client.Request.RequestMarketDataType(MarketDataType.Delayed);

            SymbolSubject
                .DistinctUntilChanged()
                .ObserveOn(Application.Current.Dispatcher)
                .Subscribe(UpdateSymbol);
        }

        private async void UpdateSymbol(string symbol)
        {
            Description = "";
            BidPrice = AskPrice = LastPrice = ChangePrice = 0;

            // Disposing the connection cancels the IB data subscription and conveniently disposes all subscriptions to the observable.
            TicksConnection.Dispose();

            if (string.IsNullOrWhiteSpace(symbol))
                return;

            Contract contract = new()
            {
                SecurityType = SecurityType.Stock,
                Symbol = symbol,
                Currency = "USD",
                Exchange = "SMART"
            };

            try
            {
                ContractDetails cd = await Client!
                    .Services
                    .CreateContractDetailsObservable(contract)
                    .FirstAsync() // Multiple may be returned. Take the first one.
                    .Timeout(TimeSpan.FromSeconds(2));

                // Display the stock name.
                Description = cd.LongName; 
            }
            catch (AlertException alertException)
            {
                MessageBox.Show($"ContractDetails: {alertException.Message}");
                return;
            }
            catch (TimeoutException)
            {
                MessageBox.Show("ContractDetails: Timeout Exception");
                return;
            }

            SubscribeToTicks(contract);
        }

        private void SubscribeToTicks(Contract contract)
        {

            // Create the observable which will emit realtime updates.
            IConnectableObservable<ITick> ticks = Client!
                .Services
                .CreateTickObservable(contract)
                .UndelayTicks()
                .ObserveOn(Application.Current.Dispatcher)
                .Publish();

            SubscribeToTicks(ticks);

            TicksConnection = ticks.Connect();
        }

        private void SubscribeToTicks(IObservable<ITick> ticks)
        {
            ticks.OfTickType(t => t.Alert).Subscribe(alert => MessageBox.Show($"Ticks: {alert.Message}")) ;

            IObservable<PriceTick> priceTicks = ticks.OfTickType(t => t.PriceTick);
            priceTicks.Where(t => t.TickType == TickType.BidPrice).Select(t => t.Price).Subscribe(p => BidPrice = p);
            priceTicks.Where(t => t.TickType == TickType.AskPrice).Select(t => t.Price).Subscribe(p => AskPrice = p);
            
            IObservable<double> lastPrices = priceTicks.Where(t => t.TickType == TickType.LastPrice).Select(t => t.Price);
            lastPrices.Subscribe(p => LastPrice = p);

            IObservable<double> changes = lastPrices.Changes();
            changes.Subscribe(p => ChangePrice = p);
            changes.ToColor().Subscribe(color => ChangeColor = color);
        }

        private async void MainWindow_OnClosing(object sender, CancelEventArgs e)
        {
            if (Client != null)
                await Client.DisposeAsync();
        }
    }
}

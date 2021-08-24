using InterReact;
using Stringification;
using System;
using System.Linq;
using System.ComponentModel;
using System.Diagnostics;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Collections.Generic;
using System.Windows.Threading;
using System.Reactive.Concurrency;

//d:DataContext="{d:DesignInstance TicksWpf:MainViewModel, IsDesignTimeCreatable=False}"

namespace TicksWpf
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
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

        private double priceChange;
        public double PriceChange
        {
            get => priceChange;
            private set
            {
                if (value == priceChange)
                    return;
                priceChange = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PriceChange)));
            }
        }

        private IInterReactClient? Client;
        private IDisposable? TicksConnection;

        public MainWindow()
        {
            InitializeComponent();

            DataContext = this;

            var t0 = Thread.CurrentThread.ManagedThreadId;
            var t1 = Application.Current.Dispatcher.Thread.ManagedThreadId;
            ;
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

            Client.Request.RequestPositions();
            int requestId = Client.Request.GetNextId();
            Client.Request.RequestAccountSummary(requestId);
            Client.Request.RequestAccountUpdates(true);

            Client.Request.RequestMarketDataType(MarketDataType.Delayed);

            //SymbolSubject
            //    .DistinctUntilChanged()
            //    .ObserveOn(Application.Current.Dispatcher)
            //    .Subscribe(UpdateSymbol);
            UpdateSymbol("SPY"); // tmp
        }

        private async void UpdateSymbol(string symbol)
        {
            Debug.WriteLine($"Symbol: {symbol}");

            Description = "";
            BidPrice = AskPrice = LastPrice = 0;

            // Disposing the connection cancels the IB data subscription and conveniently disposes all subscriptions to the observable.
            TicksConnection?.Dispose();

            if (string.IsNullOrWhiteSpace(symbol))
                return;

            Contract contract = new()
            {
                SecurityType = SecurityType.Stock,//SecurityType.Cash,
                Symbol = symbol,
                Currency = "USD",
                Exchange = "SMART"    //"IDEAL" //"IDEALPRO" //"SMART"
            };

            // Create the observable and capture the single contract details object to determine the full name of the contract.
            IObservable<Union<ContractDetails, Alert>> response = Client!
                .Services
                .CreateContractDetailsObservable(contract);

            response.Select(u => u.Source).OfType<Alert>().Subscribe(alert =>
            {
                MessageBox.Show($"ContractDetails:\n\n {alert.Message}");
            });

            IList<ContractDetails> cds = await response
                .Select(u => u.Source)
                .OfType<ContractDetails>()
                .ToList();

            // Multiple contracts may be returned. Take the first one.
            ContractDetails? cd = cds.FirstOrDefault();
            if (cd == null)
                return;

            // Display the stock name in the title bar.
            Description = cd.LongName;

            //SubscribeToTicks(contract);
        }

        private void SubscribeToTicks(Contract contract)
        {
            // Create the observable which will emit realtime updates.
            IConnectableObservable<Union<Tick,Alert>> ticks = Client!.Services
                .CreateTickObservable(contract)
                .UndelayTicks()
                .ObserveOn(Application.Current.Dispatcher)
                .Publish();

            SubscribeToTicks(ticks);

            TicksConnection = ticks.Connect();
        }

        private void SubscribeToTicks(IObservable<Union<Tick, Alert>> ticks)
        {
            // Display warnings, if any.
            ticks.Select(u => u.Source).OfType<Alert>().Subscribe(m => MessageBox.Show(m.Message));

            IObservable<PriceTick> priceTicks = ticks.OfTickType(t => t.PriceTick);

            IObservable<double> bidPrices = priceTicks.Where(t => t.TickType == TickType.BidPrice).Select(t => t.Price);
            IObservable<double> askPrices = priceTicks.Where(t => t.TickType == TickType.AskPrice).Select(t => t.Price);
            IObservable<double> lastPrices = priceTicks.Where(t => t.TickType == TickType.LastPrice).Select(t => t.Price);
            IObservable<double> lastPriceChanges = lastPrices
                .Buffer(2, 1)
                .Where(x => x.Count == 2)
                .Select(x => x[1] - x[0]);

            bidPrices.Subscribe(p => BidPrice = p);
            askPrices.Subscribe(p => AskPrice = p);
            lastPrices.Subscribe(p => LastPrice = p);
            lastPriceChanges.Subscribe(p => PriceChange = p);
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private async void MainWindow_OnClosing(object sender, CancelEventArgs e)
        {
            if (Client != null)
                await Client.DisposeAsync();
        }
    }

}

using InterReact;
using Stringification;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

//d:DataContext="{d:DesignInstance wpfDepth:MainViewModel, IsDesignTimeCreatable=False}"

namespace WpfDepth
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public MainWindow()
        {
            InitializeComponent();

            DataContext = this;

            var t0 = Thread.CurrentThread.ManagedThreadId;
            var t1 = Application.Current.Dispatcher.Thread.ManagedThreadId;
            ;
        }

        public string Symbol
        {
            set => SymbolSubject.OnNext(value);
        }
        private readonly Subject<string> SymbolSubject = new();

        public string Description { get; private set; } = "";

        public double BidPrice { get; private set; }
        public double AskPrice { get; private set; }
        public double LastPrice { get; private set; }
        private IDisposable? Subscription;
        private IInterReact? InterReactInstance;
        private IDisposable? TicksConnection;

        private async void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            var t0 = Thread.CurrentThread.ManagedThreadId;
            var t1 = Application.Current.Dispatcher.Thread.ManagedThreadId;
            ;
            try
            {
                InterReactInstance = await new InterReactBuilder().BuildAsync();
            }
            catch (Exception exception)
            {
                Utilities.MessageBoxShow(exception.Message, "InterReact", terminate: true);
                return;
            }

            InterReactInstance.Response.Subscribe(x => Debug.WriteLine(x.Stringify()));

            InterReactInstance.Request.RequestMarketDataType(MarketDataType.Delayed);

            Subscription = SymbolSubject
                .Throttle(TimeSpan.FromMilliseconds(600))
                .DistinctUntilChanged()
                .ObserveOn(Application.Current.Dispatcher)
                .Subscribe(UpdateSymbol);
        }

        private async void UpdateSymbol(string symbol)
        {
            var t0 = Thread.CurrentThread.ManagedThreadId;
            var t1 = Application.Current.Dispatcher.Thread.ManagedThreadId;

            Description = "";
            //BidPrice.Text = AskPrice.Text = LastPrice.Text = BidSize.Text = AskSize.Text = LastSize.Text = Change.Text = Volume.Text = null;
            BidPrice = AskPrice = LastPrice = 0;

            // Disposing the connection cancels the IB data subscription and conveniently disposes all subscriptions to the observable.
            TicksConnection?.Dispose();

            if (string.IsNullOrEmpty(symbol))
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
                // Create the observable and capture the single contract details object to determine the full name of the contract.
                ContractDetails cd = await InterReactInstance!.Services.CreateContractDataObservable(contract).ContractDataSingle();
                // Display the stock name in the title bar.
                Description = cd.LongName;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Description)));
            }
            catch (Exception exception)
            {
                MessageBox.Show($"ContractData exception:\n\n {exception.Message}", "InterReact");
                return;
            }

            SubscribeToTicks(contract);
        }

        private void SubscribeToTicks(Contract contract)
        {
            // Create the observable which will emit realtime updates.
            IConnectableObservable<Tick> ticks = InterReactInstance!.Services
                .CreateTickObservable(contract)
                .Undelay()
                .ObserveOn(Application.Current.Dispatcher)
                .Publish();

            SubscribeToTicks(ticks);
            TicksConnection = ticks.Connect();
        }
        private void SubscribeToTicks(IObservable<Tick> ticks)
        {
            // display warnings
            ticks.OfType<TickAlert>().Subscribe(m => MessageBox.Show(m.Alert.Message, "InterReact"));

            var priceTicks = ticks.OfType<TickPrice>();

            var bidPrices = priceTicks.Where(t => t.TickType == TickType.BidPrice).Subscribe(t =>
            {
                BidPrice = t.Price;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(BidPrice)));

            });
            var askPrices = priceTicks.Where(t => t.TickType == TickType.AskPrice).Subscribe(t =>
            {
                AskPrice = t.Price;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AskPrice)));
            });
            var lastPrices = priceTicks.Where(t => t.TickType == TickType.LastPrice).Subscribe(t =>
            {
                LastPrice = t.Price;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LastPrice)));
            });

            //askPrices.Select(p => p.ToString(priceFormat)).Subscribe(s => AskPrice.Text = s);
            //lastPrices.Select(p => p.ToString(priceFormat)).Subscribe(s => LastPrice.Text = s);

            //bidPrices.Change().ToColor().Subscribe(c => BidPrice.ForeColor = c);
            //askPrices.Change().ToColor().Subscribe(c => AskPrice.ForeColor = c);
            //lastPrices.Change().ToColor().Subscribe(c => { Change.ForeColor = LastPrice.ForeColor = c; });
            //lastPrices.Change().WhereNotNull().Select(chg => chg.ToString(priceFormat)).Subscribe(s => Change.Text = s);

            //var sizeTicks = synchronizedTicks.OfType<TickSize>();

            //const string sizeFormat = "N0";
            ///sizeTicks.Where(t => t.TickType == TickType.BidSize).Select(t => t.Size.ToString(sizeFormat)).Subscribe(s => BidSize.Text = s);
            //sizeTicks.Where(t => t.TickType == TickType.AskSize).Select(t => t.Size.ToString(sizeFormat)).Subscribe(s => AskSize.Text = s);
            //sizeTicks.Where(t => t.TickType == TickType.LastSize).Select(t => t.Size.ToString(sizeFormat)).Subscribe(s => LastSize.Text = s);
            //sizeTicks.Where(t => t.TickType == TickType.Volume).Select(t => t.Size.ToString(sizeFormat)).Subscribe(s => Volume.Text = s);
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private async void MainWindow_OnClosing(object sender, CancelEventArgs e)
        {
            if (InterReactInstance != null)
                await InterReactInstance.DisposeAsync();
        }
    }

}

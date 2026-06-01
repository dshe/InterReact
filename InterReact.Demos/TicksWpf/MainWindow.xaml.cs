using InterReact;
using Stringification;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows;
using System.Windows.Media;
namespace TicksWpf;

public partial class MainWindow : Window, INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
    private readonly Subject<string> _symbolSubject = new();

    public string Symbol
    {
        set => _symbolSubject.OnNext(value);
    }

    private string _description = "";
    public string Description
    {
        get => _description;
        private set
        {
            if (value == _description)
                return;
            _description = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Description)));
        }
    }

    private double _bidPrice;
    public double BidPrice
    {
        get => _bidPrice;
        private set
        {
            if (value == _bidPrice)
                return;
            _bidPrice = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(BidPrice)));
        }
    }

    private double _askPrice;
    public double AskPrice
    {
        get => _askPrice;
        private set
        {
            if (value == _askPrice)
                return;
            _askPrice = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AskPrice)));
        }
    }

    private double _lastPrice;
    public double LastPrice
    {
        get => _lastPrice;
        private set
        {
            if (value == _lastPrice)
                return;
            _lastPrice = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LastPrice)));
        }
    }

    private double _changePrice;
    public double ChangePrice
    {
        get => _changePrice;
        private set
        {
            if (value == _changePrice)
                return;
            _changePrice = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ChangePrice)));
        }
    }

    private SolidColorBrush _changeColor = Brushes.Transparent;
    public SolidColorBrush ChangeColor
    {
        get => _changeColor;
        private set
        {
            if (value == _changeColor)
                return;
            _changeColor = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ChangeColor)));
        }
    }

    ////////////////////////////////////////////////////////////////////

    private IInterReactClient _client = NullInterReactClient.Instance;
    private IDisposable _ticksConnection = Disposable.Empty;

    public MainWindow()
    {
        InitializeComponent();
        DataContext = this;
    }

    private async void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
    {
        try
        {
            _client = await InterReactClient.CreateAsync(options => options.UseDelayedTicks = false);
        }
        catch (Exception exception)
        {
            Utilities.MessageBoxShow(exception.Message, "InterReact", terminate: true);
            return;
        }

        // Display response messages to the debug window.
        _client.Response.Subscribe(
            msg => Debug.WriteLine(msg.Stringify()),
            ex => Utilities.MessageBoxShow(ex.Message, "InterReact"));

        // Use delayed market data in case there is no real-time data subscription.
        // Delayed data is notmally received through delayed data ticks. 
        // However, delayed data is received through notmal ticks due to the setting above: "options.UseDelayedTicks = false"
        await _client.Request.RequestMarketDataTypeAsync(MarketDataType.Delayed);

        // Subscribe to changes of the text in the SymbolTextBox.
        _symbolSubject
            .Throttle(TimeSpan.FromMilliseconds(500))
            .DistinctUntilChanged()
            .ObserveOnDispatcher()
            .Subscribe(UpdateSymbolAsync);
    }

    private async void UpdateSymbolAsync(string symbol)
    {
        Description = "";
        BidPrice = AskPrice = LastPrice = ChangePrice = 0;

        // Disposing the connection cancels the previous IB data subscription
        // and conveniently disposes all subscriptions to the observable (below).
        _ticksConnection.Dispose();

        if (string.IsNullOrWhiteSpace(symbol))
            return;

        Contract contract = new()
        {
            Symbol = symbol,
            SecurityType = ContractSecurityType.Stock,
            Currency = "USD",
            Exchange = "SMART"
        };

        IHasRequestId message = await _client.Service
            .CreateContractDetailsObservable(contract)
            .FirstAsync();

        if (message is ContractDetails cd)
        {
            Description = cd.LongName;
            SubscribeToTicks(cd.Contract);
        }
        else if (message is Alert alert)
            Utilities.MessageBoxShow(alert.Message, "InterReact");
        else
            Utilities.MessageBoxShow("Unhandled type: " + message.GetType().Name + "", "InterReact");
    }

    private void SubscribeToTicks(Contract contract)
    {
        // Create a connectable observable which will emit updates.
        IConnectableObservable<IHasRequestId> ticks = _client
            .Service
            .CreateMarketDataObservable(contract)
            .Publish();

        ticks.Subscribe(onNext: _ => { }, onError: exception => MessageBox.Show($"Fatal: {exception.Message}"));
        ticks.OfTickClass(t => t.Alert)
            .Where(alert => alert.Code != AlertDefinition.MarketDataNotSubscribed.Code)
            .Subscribe(alert => MessageBox.Show($"{alert.Message}"));

        IObservable<PriceTick> priceTicks = ticks.OfTickClass(t => t.PriceTick);
        priceTicks.Where(t => t.TickType == TickType.BidPrice || t.TickType == TickType.DelayedBidPrice)
            .Select(t => t.Price).Subscribe(p => BidPrice = p);
        priceTicks.Where(t => t.TickType == TickType.AskPrice || t.TickType == TickType.DelayedAskPrice)
            .Select(t => t.Price).Subscribe(p => AskPrice = p);

        IObservable<double> lastPrices = priceTicks
            .Where(t => t.TickType == TickType.LastPrice || t.TickType == TickType.DelayedLastPrice)
            .Select(t => t.Price);
        lastPrices.Subscribe(p => LastPrice = p);

        IObservable<double> changes = lastPrices.Changes();
        changes.Subscribe(p => ChangePrice = p);
        changes.ToColor().Subscribe(color => ChangeColor = color);

        // Activate the subsciptions to the observable.
        _ticksConnection = ticks.Connect();
    }

    private async void MainWindow_OnClosing(object sender, CancelEventArgs e)
    {
        _ticksConnection.Dispose();
        await _client.DisposeAsync();
    }

}

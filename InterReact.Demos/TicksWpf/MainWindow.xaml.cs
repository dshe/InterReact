using InterReact;
using Stringification;
using System;
using System.Linq;
using System.ComponentModel;
using System.Diagnostics;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows;
using System.Reactive.Disposables;
using System.Windows.Media;

namespace TicksWpf;

public partial class MainWindow : Window, INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    private readonly Subject<string> SymbolSubject = new();
    public string Symbol
    {
        set => SymbolSubject.OnNext(value);
    }

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
            Client = await new InterReactClientConnector().ConnectAsync();
        }
        catch (Exception exception)
        {
            Utilities.MessageBoxShow(exception.Message, "InterReact", terminate: true);
            return;
        }

        // display response messages to the debug window.
        Client.Response.Subscribe(msg => Debug.WriteLine(msg.Stringify()));

        Client.Request.RequestMarketDataType(MarketDataType.Delayed);

        // Subscribe to changes of the text in the SymbolTextBox.
        SymbolSubject
            .DistinctUntilChanged()
            .Subscribe(UpdateSymbol);
    }

    private async void UpdateSymbol(string symbol)
    {
        // Note that this is the UI thread.
        Application.Current.Dispatcher.VerifyAccess();

        Description = "";
        BidPrice = AskPrice = LastPrice = ChangePrice = 0;

        // Disposing the connection cancels the IB data subscription and conveniently disposes all subscriptions to the observable.
        TicksConnection.Dispose();

        if (string.IsNullOrWhiteSpace(symbol))
            return;

        Contract contract = new()
        {
            Symbol = symbol,
            SecurityType = SecurityType.Stock,
            Currency = "USD",
            Exchange = "SMART"
        };

        try
        {
            ContractDetails cd = await Client!
                .Service
                .CreateContractDetailsObservable(contract)
                .Timeout(TimeSpan.FromSeconds(2))
                .FirstAsync(); // Multiple may be returned. Take the first one.

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
            MessageBox.Show("ContractDetails: Timeout.");
            return;
        }

        SubscribeToTicks(contract);
    }

    private void SubscribeToTicks(Contract contract)
    {
        // Create the observable which will emit realtime updates.
        IConnectableObservable<ITick> ticks = Client!
            .Service
            .CreateTickObservable(contract)
            .UndelayTicks()
            .ObserveOnDispatcher()
            .Publish();

        SubscribeToTicks(ticks);

        TicksConnection = ticks.Connect();
    }

    private void SubscribeToTicks(IObservable<ITick> ticks)
    {
        ticks.Subscribe(onNext: _ => { }, onError: exception => MessageBox.Show($"Fatal: {exception.Message}"));
        ticks.OfTickClass(t => t.Alert).Subscribe(alert => MessageBox.Show($"{alert.Message}"));

        IObservable<PriceTick> priceTicks = ticks.OfTickClass(t => t.PriceTick);
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
        if (Client is not null)
            await Client.DisposeAsync();
    }
}

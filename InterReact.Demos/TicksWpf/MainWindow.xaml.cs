﻿using InterReact;
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

    private IInterReactClient Client = NullInterReactClient.Instance;
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
            Client = await InterReactClient.ConnectAsync();
        }
        catch (Exception exception)
        {
            Utilities.MessageBoxShow(exception.Message, "InterReact", terminate: true);
            return;
        }

        // display response messages to the debug window.
        Client.Response.Subscribe(
            msg => Debug.WriteLine(msg.Stringify()),
            //ex => Utilities.MessageBoxShow(ex.Message, "InterReact"));
            ex => Debug.WriteLine(ex.Message));

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
            SecurityType = ContractSecurityType.Stock,
            Currency = "USD",
            Exchange = "SMART"
        };

        ContractDetails cd = await Client.Service
            .CreateContractDetailsObservable(contract)
            .Timeout(TimeSpan.FromSeconds(5))
            .Catch<ContractDetails, Exception>(ex =>
            {
                // Could be symbol format, not found, error or timeout.
                Utilities.MessageBoxShow(ex.Message, "InterReact");
                return Observable.Empty<ContractDetails>();
            })
            .FirstOrDefaultAsync();

        if (cd == null)
            return;

        // Display the stock name.
        Description = cd.LongName;

        SubscribeToTicks(contract);
    }

    private void SubscribeToTicks(Contract contract)
    {
        // Create a connectable observable which will emit realtime updates.
        IConnectableObservable<IHasRequestId> ticks = Client
            .Service
            .CreateMarketDataObservable(contract)
            .ObserveOnDispatcher()
            .Publish();

        ticks.Subscribe(onNext: _ => { }, onError: exception => MessageBox.Show($"Fatal: {exception.Message}"));
        ticks.OfTickClass(t => t.Alert).Subscribe(alert => MessageBox.Show($"{alert.Message}"));

        IObservable<PriceTick> priceTicks = ticks.OfTickClass(t => t.PriceTick);
        priceTicks.Where(t => t.TickType == TickType.BidPrice || t.TickType == TickType.DelayedBidPrice)
            .Select(t => t.Price).Subscribe(p => BidPrice = p);
        priceTicks.Where(t => t.TickType == TickType.AskPrice || t.TickType == TickType.DelayedAskPrice)
            .Select(t => t.Price).Subscribe(p => AskPrice = p);

        IObservable<double> lastPrices = priceTicks.Where(t => t.TickType == TickType.LastPrice || t.TickType == TickType.DelayedLastPrice)
            .Select(t => t.Price);
        lastPrices.Subscribe(p => LastPrice = p);

        IObservable<double> changes = lastPrices.Changes();
        changes.Subscribe(p => ChangePrice = p);
        changes.ToColor().Subscribe(color => ChangeColor = color);

        // Activate the subsciptions to the observable.
        TicksConnection = ticks.Connect();
    }

    private async void MainWindow_OnClosing(object sender, CancelEventArgs e) => await Client.DisposeAsync();
}


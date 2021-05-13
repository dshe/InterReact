using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Reactive.Linq;
using System.Threading;
using InterReact;
using InterReact.Extensions;
using System.Diagnostics;
using Stringification;

namespace WinFormsTicks
{
    public partial class Form1 : Form
    {
        private readonly SynchronizationContext synchronizationContext;
        private IInterReactClient? client;
        private IDisposable? connection;

        public Form1()
        {
            InitializeComponent();
            synchronizationContext = SynchronizationContext.Current ?? throw new NullReferenceException("SynchronizationContext");
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                client = await new InterReactClientBuilder().BuildAsync().ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                ShowMessage(exception.Message, close: true);
                return;
            }

            // Write all incoming messages to the debug window to see what's happening.
            // Also observe any exceptions.
            client.Response.StringifyItems().Subscribe(Console.WriteLine, exception => ShowMessage(exception.Message));


            //crash
            SymbolLabel.Visible = Symbol.Visible = true;

            // Listen for textbox TextChanged messages to determine the symbol.
            Observable.FromEventPattern(Symbol, "TextChanged")
                .Select(ea => ((TextBox)ea.Sender!).Text)
                .Throttle(TimeSpan.FromSeconds(1))
                .DistinctUntilChanged()
                .ObserveOn(synchronizationContext)
                .Subscribe(UpdateSymbol);
        }

        private async void UpdateSymbol(string symbol)
        {
            Text = "InterReact";
            BidPrice.Text = AskPrice.Text = LastPrice.Text = BidSize.Text = AskSize.Text = LastSize.Text = Change.Text = Volume.Text = null;

            // Disposing the connection cancels the IB data subscription and conveniently disposes all subscriptions to the observable.
            connection?.Dispose();

            if (string.IsNullOrEmpty(symbol))
                return;

            var contract = new Contract
            {
                SecurityType = SecurityType.Stock,
                Symbol = symbol,
                Currency = "USD",
                Exchange = "SMART"
            };

            try
            {
                // Create the observable and capture the single contract details object to determine the full name of the contract.
                var contractData = await client!.Services.CreateContractDataObservable(contract).ContractDataSingle();
                // Display the stock name in the title bar.
                Text = $"{contractData.LongName} ({symbol})";
            }
            catch (Exception exception)
            {
                MessageBox.Show($"ContractData exception:\n\n {exception.Message}", "InterReact");
                return;
            }

            client.Request.RequestMarketDataType(MarketDataType.Delayed);

            // Create the object containing the observable which will emit realtime updates.
            var ticks = client.Services.CreateTickConnectableObservable(contract);

            SubscribeToTicks(ticks.Undelay().ObserveOn(synchronizationContext));

            connection = ticks.Connect();
        }

        private void SubscribeToTicks(IObservable<Tick> synchronizedTicks)
        {
            // Observe any errors.
            synchronizedTicks.Subscribe(m => { }, ex => ShowMessage(ex.Message));

            var priceTicks = synchronizedTicks.OfType<TickPrice>();
            priceTicks.Subscribe(x => Debug.WriteLine("Here is price: " + x.Price + " " + x.TickType));

            var bidPrices = priceTicks.Where(t => t.TickType == TickType.BidPrice).Select(t => t.Price);
            var askPrices = priceTicks.Where(t => t.TickType == TickType.AskPrice).Select(t => t.Price);
            var lastPrices = priceTicks.Where(t => t.TickType == TickType.LastPrice).Select(t => t.Price);

            //lastPrices.Subscribe(x => Debug.WriteLine("Here is price: " + x));

            const string priceFormat = "N2";
            bidPrices.Select(p => p.ToString(priceFormat)).Subscribe(s => BidPrice.Text = s);
            askPrices.Select(p => p.ToString(priceFormat)).Subscribe(s => AskPrice.Text = s);
            lastPrices.Select(p => p.ToString(priceFormat)).Subscribe(s => LastPrice.Text = s);

            bidPrices.Change().ToColor().Subscribe(c => BidPrice.ForeColor = c);
            askPrices.Change().ToColor().Subscribe(c => AskPrice.ForeColor = c);
            lastPrices.Change().ToColor().Subscribe(c => { Change.ForeColor = LastPrice.ForeColor = c; });
            lastPrices.Change().WhereHasValue().Select(chg => chg.ToString(priceFormat)).Subscribe(s => Change.Text = s);

            var sizeTicks = synchronizedTicks.OfType<TickSize>();

            const string sizeFormat = "N0";
            sizeTicks.Where(t => t.TickType == TickType.BidSize).Select(t => t.Size.ToString(sizeFormat)).Subscribe(s => BidSize.Text = s);
            sizeTicks.Where(t => t.TickType == TickType.AskSize).Select(t => t.Size.ToString(sizeFormat)).Subscribe(s => AskSize.Text = s);
            sizeTicks.Where(t => t.TickType == TickType.LastSize).Select(t => t.Size.ToString(sizeFormat)).Subscribe(s => LastSize.Text = s);
            sizeTicks.Where(t => t.TickType == TickType.Volume).Select(t => t.Size.ToString(sizeFormat)).Subscribe(s => Volume.Text = s);
        }

        private void ShowMessage(string message, bool close = false)
        {
            RunSynchronized(() =>
            {
                MessageBox.Show(message);
                if (close)
                    Close();
            });
        }

        private void RunSynchronized(Action action)
        {
            if (synchronizationContext == SynchronizationContext.Current)
                action();
            else
                synchronizationContext.Post(x => action(), null);
        }

        private async void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (client != null)
                await client.DisposeAsync();
        }
    }

    internal static class UtilityEx
    {
        internal static IObservable<Color> ToColor(this IObservable<double?> source)
        {
            return source.Select(change =>
            {
                if (change == null) // no previous value available
                    return Color.DarkGray;
                if (change > 0)
                    return Color.Green;
                if (change < 0)
                    return Color.Red;
                return Color.White;
            });
        }
    }
}


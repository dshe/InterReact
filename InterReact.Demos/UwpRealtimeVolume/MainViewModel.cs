using System;
using System.Diagnostics;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Core;
using InterReact;
using InterReact.Enums;
using InterReact.Extensions;
using InterReact.Interfaces;
using InterReact.Messages;
using InterReact.StringEnums;
using UwpRealtimeVolume.Utility;
using NodaTime;

namespace UwpRealtimeVolume
{
    public class MainViewModel
    {
        private const string PriceFormat = "{0:#,0.00;-#;#}";
        private const string PriceChangeFormat = "{0:+##0.00;-##0.00;##0.00}";
        private const string SizeFormat = "{0:#,#}";
        public readonly BindableObserver<string> Title = new BindableObserver<string>();
        public readonly BindableObserver<DateTime> Date = new BindableObserver<DateTime>();
        public readonly TextBoxChangingObservable Symbol = new TextBoxChangingObservable();
        public readonly BindableObserver<double> BidPrice = new BindableObserver<double>(PriceFormat);
        public readonly BindableObserver<double> AskPrice = new BindableObserver<double>(PriceFormat);
        public readonly BindableObserver<double> LastPrice = new BindableObserver<double>(PriceFormat);
        public readonly BindableObserver<double?> BidPriceChange = new BindableObserver<double?>(PriceChangeFormat);
        public readonly BindableObserver<double?> AskPriceChange = new BindableObserver<double?>(PriceChangeFormat);
        public readonly BindableObserver<double?> LastPriceChange = new BindableObserver<double?>(PriceChangeFormat);
        public readonly BindableObserver<double?> Spread = new BindableObserver<double?>("Spread: {0:##0.0;-##0.0;0}bp");
        public readonly BindableObserver<int> BidSize = new BindableObserver<int>(SizeFormat);
        public readonly BindableObserver<int> AskSize = new BindableObserver<int>(SizeFormat);
        public readonly BindableObserver<int> LastSize = new BindableObserver<int>(SizeFormat);
        public readonly BindableObserver<int> Volume = new BindableObserver<int>(SizeFormat);
        public readonly BindableObserver<string> ContractTimeStatus = new BindableObserver<string>();
        //public readonly BindableObserver<DateTime> LastTime = new BindableObserver<DateTime>("{0:yyyy-MM-dd HH:mm:ss}");
        public readonly BindableObserver<Instant> LastInstant = new BindableObserver<Instant>("{0:yyyy-MM-dd HH:mm:ss}");
        private IInterReactClient interReactClient;
        private IDisposable ticksSubscription;

        public MainViewModel()
        {
            Title.Value = "InterReact";
            if (!DesignMode.DesignModeEnabled)
               Init();
        }

        public async void Init()
        {
            Observable.Timer(DateTimeOffset.Now, TimeSpan.FromSeconds(1)).Subscribe(x => Date.Value = DateTime.Now);
            try
            {
                interReactClient = await InterReactClient.BuildAsync();
            }
            catch (Exception exception)
            {
                await SyncMessageBox.Show(exception.Message, terminate:true);
                return;
            }
            CoreApplication.Suspending += (x, y) => { interReactClient.Dispose(); };

            // TickRedundantRealtimeVolumeFilter removes redundant messages (LastPrice, LastSize, Volume and Time ticks).
            // Display a message box for any errors. Write messages to the Debug window.
            interReactClient.Response
                .Subscribe(async message =>
                {
                    Debug.WriteLine(message.Stringify());
                    if (message is Alert alert &&
                        (alert.AlertType == AlertType.ConnectionLost || alert.AlertType == AlertType.ConnectionRestored))
                            await SyncMessageBox.Show(alert.Message);
                }, async exception => await SyncMessageBox.Show(exception.Message, terminate:true));

            // Note that Symbol is an observable.
            Symbol.Throttle(TimeSpan.FromSeconds(1))
                .DistinctUntilChanged()
                .Subscribe(async symbol =>
                {
                    try
                    {
                        await FindContract(symbol);
                    }
                    catch (Exception exception)
                    {
                        await SyncMessageBox.Show(exception.Message);
                    }
                });
        }

        private async Task FindContract(string symbol)
        {
            // Dispose to cancel the IB data subscription and dispose all subscriptions.
            ticksSubscription?.Dispose();

            BidPrice.Value = AskPrice.Value = LastPrice.Value = 0;
            BidSize.Value = AskSize.Value = LastSize.Value = Volume.Value = 0;
            LastPriceChange.Value = Spread.Value = null;
            ContractTimeStatus.Value = null;
            LastInstant.Value = default;
            Title.Value = "InterReact";

            if (string.IsNullOrEmpty(symbol)) { return; }

            var xxx = SecurityType.Stock;

            var contract = new Contract { SecurityType = SecurityType.Stock, Symbol = symbol, Currency = "USD", Exchange = "SMART" };

            // Find the contract details and use it to display the full name of the contract in the window title bar.
            var contractDetailsList = interReactClient.Services.ContractDetailsObservable(contract);
            var contractDetails = await contractDetailsList.ContractDetailsSingle();
            Title.Value = contractDetails.LongName + " (" + symbol + ")";

            // Find the current and next time status for the contract.
            var cdt = new InterReact.Utility.ContractDetailsTime(contractDetails);
            var status = cdt.Get();
            var current = status.Previous;
            var next = status.Next;
            ContractTimeStatus.Value = $"Status: {current.Status}, changing to {next.Status} at: {next.Time:yyyy-MM-dd HH:mm:ss}";
            
            // Create the observable which will emit tick updates.
            // Specify that RealTimeVolume ticks be emitted rather than LastPrice, LastSize, Volume and Time messages.
            var ticks = interReactClient.Services.TickConnectableObservable(contract, new[] { GenericTickType.RealtimeVolume }, true);

            SubscribeToTicks(ticks);

            // Make the request to IB to start receiving ticks.
            ticksSubscription = ticks.Connect();
        }

        private void SubscribeToTicks(IObservable<ITick> ticks)
        {
            // Display error messages, if any.
            ticks.Subscribe(m => { }, async exception => await SyncMessageBox.Show(exception.Message));

            var bidAskTicks = ticks.ToBidAskTicks();
            bidAskTicks.Select(y => y.Spread()).Subscribe(Spread);

            var bidPrices = bidAskTicks.Select(tick => tick.BidPrice);
            bidPrices.Subscribe(BidPrice);
            bidPrices.Change().Subscribe(BidPriceChange);

            var askPrices = bidAskTicks.Select(tick => tick.AskPrice);
            askPrices.Subscribe(AskPrice);
            askPrices.Change().Subscribe(AskPriceChange);

            var sizeTicks = ticks.OfType<TickSize>();
            sizeTicks.Where(x => x.TickType == TickType.BidSize).Select(x => x.Size).Subscribe(BidSize);
            sizeTicks.Where(x => x.TickType == TickType.AskSize).Select(x => x.Size).Subscribe(AskSize);

            var realtimeTicks = ticks.OfType<TickRealtimeVolume>();

            var prices = realtimeTicks.Select(tick => tick.Price);
            prices.Subscribe(LastPrice);
            prices.Change().Subscribe(LastPriceChange);

            realtimeTicks.Select(tick => tick.Size).Subscribe(LastSize);
            realtimeTicks.Select(m => m.Volume).Subscribe(Volume);
            realtimeTicks.Select(m => m.Instant).Subscribe(LastInstant);
        }
    }
}

using System;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using InterReact;
using InterReact.Messages;
using InterReact.StringEnums;
using InterReact.Extensions;
using System.Windows.Data;

namespace WpfDepth
{
    public sealed class MainViewModel : NotifyPropertyChangedBase, IDisposable
    {
        private readonly object Gate = new object();
        public string Title { get; private set; } = "InterReact";
        public ReadOnlyObservableCollection<MarketDepthItem> Bids { get; private set; }
        public ReadOnlyObservableCollection<MarketDepthItem> Asks { get; private set; }

        private IInterReactClient client;

        public async Task Initialize()
        {
            try
            {
                client = await InterReactClient.Builder.BuildAsync();
            }
            catch (Exception exception)
            {
                Utilities.MessageBoxShow(exception.Message, "InterReact", terminate: true);
                return;
            }

            // Create a contract. Note that market depth is not available for the demo account.
            var contract = new Contract
            {
                SecurityType = SecurityType.Cash,
                Symbol = "EUR",
                Currency = "USD",
                Exchange = "IDEALPRO"
            };

            Title = contract.Currency + "/" + contract.Symbol;

            var depthObservable = client.Services
                 .MarketDepthObservable(contract, rows: 5);

            var (Bids, Asks) = depthObservable.ToMarketDepthObservableCollections();
            //.ObserveOn(SynchronizationContext.Current)
            //.ObserveOnDispatcher()

            // Observe any exceptions.
            depthObservable.Subscribe(m => { }, e => Utilities.MessageBoxShow(e.Message, "InterReact", terminate: true));

            // Notify UI that all (2) properties changed.
            NotifyPropertiesChanged();

            depthObservable.Connect();
        }

        public void Dispose() => client?.Dispose();
    }
}

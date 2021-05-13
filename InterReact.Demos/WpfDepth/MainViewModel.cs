using System;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using System.Threading.Tasks;
using InterReact;


using InterReact.Extensions;

namespace WpfDepth
{
    public sealed class MainViewModel : NotifyPropertyChangedBase, IAsyncDisposable
    {
        //private readonly object Gate = new object();
        public string Title { get; private set; } = "InterReact";
        public ReadOnlyObservableCollection<MarketDepthItem>? Bids { get; private set; }
        public ReadOnlyObservableCollection<MarketDepthItem>? Asks { get; private set; }

        private IInterReactClient? client;

        public async Task Initialize()
        {
            try
            {
                client = await new InterReactClientBuilder().BuildAsync();
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
                 .CreateMarketDepthObservable(contract, rows: 5);

            var (Bids, Asks) = depthObservable.ToMarketDepthObservableCollections();
            //.ObserveOn(SynchronizationContext.Current)
            //.ObserveOnDispatcher()

            // Observe any exceptions.
            depthObservable.Subscribe(m => { }, e => Utilities.MessageBoxShow(e.Message, "InterReact", terminate: true));

            // Notify UI that all (2) properties changed.
            NotifyPropertiesChanged();

            depthObservable.Connect();
        }

        public async ValueTask DisposeAsync()
        {
            if (client != null)
                await client.DisposeAsync();
        }
    }
}

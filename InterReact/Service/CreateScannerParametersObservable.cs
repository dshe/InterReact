using System;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using InterReact.Extensions;


namespace InterReact
{
    public sealed partial class Services
    {
        /// <summary>
        /// An observable which, upon subscription, emits scanner parameters, then completes.
        /// </summary>
        public IObservable<string> CreateScannerParametersObservable() =>
            Response
                .ToObservable<ScannerParameters>(Request.RequestScannerParameters)
                .Select(m => m.Parameters)
                .ToShareSource();
    }
}

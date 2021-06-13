using System;
using System.Reactive.Linq;

namespace InterReact
{
    public sealed partial class Services
    {
        /// <summary>
        /// An observable which, upon subscription, emits scanner parameters, then completes.
        /// </summary>
        public IObservable<string> CreateScannerParametersObservable() =>
            Response
                .ToObservableSingle<ScannerParameters>(Request.RequestScannerParameters)
                .Select(m => m.Parameters)
                .ToShareSource();
    }
}

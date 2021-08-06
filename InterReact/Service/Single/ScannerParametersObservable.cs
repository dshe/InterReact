using System;
using System.Reactive.Linq;

namespace InterReact
{
    public partial class Services
    {
        /// <summary>
        /// An observable which, upon subscription, emits scanner parameters, then completes.
        /// </summary>
        public IObservable<string> ScannerParametersObservable { get; }

        private IObservable<string> CreateScannerParametersObservable() =>
            Response
                .ToObservableSingle<ScannerParameters>(Request.RequestScannerParameters)
                .Select(m => m.Parameters)
                .ShareSource();
    }
}

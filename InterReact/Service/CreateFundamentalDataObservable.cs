using System;
using System.Reactive.Linq;

namespace InterReact
{
    public sealed partial class Services
    {
        /// <summary>
        /// Creates an observable which emits fundamental data for the company represented by the contract, then completes.
        /// </summary>
        public IObservable<string> CreateFundamentalDataObservable(Contract contract, FundamentalDataReportType? reportType = null) =>
            Response
                .ToObservableWithIdSingle<FundamentalData>(
                    Request.GetNextId,
                    id => Request.RequestFundamentalData(id, contract, reportType),
                    Request.CancelFundamentalData)
                .Select(m => m.Data)
                .ToShareSource();
    }
}

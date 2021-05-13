using System;
using System.Reactive.Linq;

using InterReact.Extensions;


namespace InterReact
{
    public sealed partial class Services
    {
        /// <summary>
        /// Creates an observable which emits fundamental data for the company represented by the contract, then completes.
        /// </summary>
        public IObservable<string> CreateFundamentalDataObservable(Contract contract, FundamentalDataReportType? reportType = null)
        {
            return Response
                .ToObservableWithId<FundamentalData>(
                    Request.GetNextId,
                    requestId => Request.RequestFundamentalData(requestId, contract, reportType),
                    Request.CancelFundamentalData)
                .Select(m => m.Data)
                .ToShareSource();
        }

    }
}

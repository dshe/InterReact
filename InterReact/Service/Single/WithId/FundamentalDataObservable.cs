using System;
using System.Reactive.Linq;

namespace InterReact
{
    public sealed partial class Services
    {
        /// <summary>
        /// Creates an observable which emits fundamental data for the company represented by the contract, then completes.
        /// Each subscription makes a separate request.
        /// </summary>
        public IObservable<Union<FundamentalData, Alert>> CreateFundamentalDataObservable(Contract contract, FundamentalDataReportType? reportType = null) =>
            Response
                .ToObservableSingleWithId(
                    Request.GetNextId,
                    id => Request.RequestFundamentalData(id, contract, reportType),
                    Request.CancelFundamentalData)
                .Select(x => new Union<FundamentalData, Alert>(x));
    }
}

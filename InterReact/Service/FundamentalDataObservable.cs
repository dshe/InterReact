using System;
using System.Reactive.Linq;

namespace InterReact
{
    public sealed partial class Services
    {
        /// <summary>
        /// Creates an observable which emits fundamental data for the company represented by the contract, then completes.
        /// </summary>
        public IObservable<IFundamentalData> CreateFundamentalDataObservable(Contract contract, FundamentalDataReportType? reportType = null) =>
            Response
                .ToObservableWithIdSingle<IFundamentalData>(
                    Request.GetNextId,
                    id => Request.RequestFundamentalData(id, contract, reportType),
                    Request.CancelFundamentalData)
                .ToShareSource();
    }

    public static partial class Extensions
    {
        public static IObservable<FundamentalData> OfTypeFundamentalData(this IObservable<IFundamentalData> source) =>
            source.OfType<FundamentalData>();
    }

}

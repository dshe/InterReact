using System;
using System.Reactive.Linq;

namespace InterReact
{
    public sealed partial class Services
    {
        /// <summary>
        /// Creates an observable which, upon subscription, emits the specified financial advisor object, then completes.
        /// Note that the request will return an error if the account is not a financial advisor account.
        /// </summary>
        public IObservable<string> CreateFinancialAdvisorConfigurationObservable(FinancialAdvisorDataType type) =>
            Response
                .ToObservableSingle<FinancialAdvisor>(() => Request.RequestFinancialAdvisorConfiguration(type))
                .Select(x => x.XmlData)
                .ShareSource();
    }
}

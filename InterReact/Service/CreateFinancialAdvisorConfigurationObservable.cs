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
        public IObservable<FinancialAdvisor> CreateFinancialAdvisorConfigurationObservable(FinancialAdvisorDataType type) =>

            Observable.Create<FinancialAdvisor>(observer =>
            {
                var subscription = Response.Subscribe(m =>
                {
                    if (m is FinancialAdvisor fa && fa.DataType == type)
                    {
                        observer.OnNext(fa);
                        observer.OnCompleted();
                    }
                    else if (m is Alert alert && alert.Code == 321)
                        observer.OnError(alert.ToException());

                }, observer.OnError, observer.OnCompleted);

                Request.RequestFinancialAdvisorConfiguration(type);

                return subscription;

            }).ToShareSource();
    }
}

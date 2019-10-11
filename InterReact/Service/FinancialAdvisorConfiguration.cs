using System;
using InterReact.Core;
using InterReact.Interfaces;
using System.Reactive.Linq;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using InterReact.Enums;
using InterReact.Messages;
using InterReact.StringEnums;
using InterReact.Utility;
using InterReact.Utility.Rx;

namespace InterReact.Service
{
    public sealed partial class Services
    {
        /// <summary>
        /// Creates an observable which, upon subscription, emits the specified financial advisor object, then completes.
        /// Note that the request will return an error if the account is not a financial advisor account.
        /// </summary>
        public IObservable<FinancialAdvisor> FinancialAdvisorConfigurationObservable(FinancialAdvisorDataType type)
        {
            return Observable.Create<FinancialAdvisor>(observer =>
            {
                var subscription = Response
                    .Subscribe(m =>
                    {
                        if (m is FinancialAdvisor fa && fa.DataType == type)
                        {
                            observer.OnNext(fa);
                            observer.OnCompleted();
                        }
                        else if (m is Alert alert && alert.Code == 321)
                            observer.OnError(alert);

                    }, observer.OnError, observer.OnCompleted);

                Request.RequestFinancialAdvisorConfiguration(type);

                return subscription;

            }).ToShareSource();
        }
    }

}

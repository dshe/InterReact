﻿using System;

namespace InterReact
{
    public sealed partial class Services
    {
        /// <summary>
        /// Creates an observable which, upon subscription, emits the specified financial advisor object, then completes.
        /// Note that the request will return an error if the account is not a financial advisor account.
        /// </summary>
        public IObservable<FinancialAdvisor> CreateFinancialAdvisorConfigurationObservable(FinancialAdvisorDataType type) =>
            Response
                .ToObservableSingle<FinancialAdvisor>(() => Request.RequestFinancialAdvisorConfiguration(type))
                .ShareSource();
    }
}

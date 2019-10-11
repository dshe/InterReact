using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reactive.Linq;
using System.Threading;
using InterReact.Core;
using InterReact.StringEnums;
using InterReact.Interfaces;
using InterReact.Messages;
using InterReact.Utility.Rx;

#nullable enable

namespace InterReact.Service
{
    public sealed partial class Services
    {
        /// <summary>
        /// Creates an observable which emits fundamental data for the company represented by the contract, then completes.
        /// </summary>
        public IObservable<string> FundamentalDataObservable(Contract contract, FundamentalDataReportType? reportType = null)
        {
            if (contract == null)
                throw new ArgumentNullException(nameof(contract));

            return Response
                .ToObservable<FundamentalData>(
                    Request.NextId,
                    requestId => Request.RequestFundamentalData(requestId, contract, reportType),
                    Request.CancelFundamentalData)
                .Select(m => m.Data)
                .ToShareSource();
        }

    }
}

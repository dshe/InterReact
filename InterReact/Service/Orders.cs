using System;
using System.Reactive.Linq;
using System.Threading;
using InterReact.Core;
using InterReact.Enums;
using InterReact.Interfaces;
using InterReact.Messages;
using InterReact.Service.Utility;
using InterReact.Utility.Rx;

namespace InterReact.Service
{
    public sealed partial class Services
    {
        /// <summary>
        /// Creates an observable which, upon subscription, returns open orders and order status reports, then completes.
        /// Multiple simultaneous observers among different OpenOrderRequestTypes is not supported.
        /// </summary>
        public IObservable<IHasOrderId> OpenOrdersObservable(OpenOrdersRequestType type = OpenOrdersRequestType.AllOpenOrders)
        {
            Action subscribe;
            switch (type)
            {
                case OpenOrdersRequestType.OpenOrders:
                    subscribe = Request.RequestOpenOrders;
                    break;
                case OpenOrdersRequestType.AllOpenOrders:
                    subscribe = Request.RequestAllOpenOrders;
                    break;
                case OpenOrdersRequestType.AutoOpenOrders:
                    subscribe = () => Request.RequestAutoOpenOrders(false);
                    break;
                case OpenOrdersRequestType.AutoOpenOrdersWithBind:
                    subscribe = () => Request.RequestAutoOpenOrders(true);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
            return Response
                .Where(m => m is OpenOrder || m is OrderStatusReport || m is OpenOrderEnd)
                .ToObservable<IHasOrderId>(subscribe, null, m => m is OpenOrderEnd)
                .ToShareSource();
        }

        /// <summary>
        /// An observable which emits Execution and CommissionReport objects, then completes.
        /// </summary>
        internal IObservable<IHasRequestId> ExecutionAndCommissionsObservable =>
            Response
            .ToObservable<IHasRequestId>(Request.NextId, requestId => Request.RequestExecutions(requestId), null, m => m is ExecutionEnd)
            .ToShareSource();

        /// <summary>
        /// Places an order and returns a object use to monitor the order.
        /// </summary>
        public OrderMonitor PlaceOrder(Order order, Contract contract, int orderId = 0) =>
            new OrderMonitor(Request, Response, order, contract, orderId, Config.Clock);
    }

}

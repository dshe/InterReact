using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using NodaTime;

namespace InterReact.Utility
{
    /// <summary>
    /// This object is returned from Services.PlaceOrder(...).
    /// It contains references to the order information and provides a method to cancel the order.
    /// It also provides an observable which relays order messages (OpenOrder, OrderStatusReport, Execution, CommissionReport and possibly Alert).
    /// Results are cached and replayed to subscribers.
    /// This observable completes only when the object is disposed.
    /// Use Take(Timespan) operator to return an observable that contains the latest Values.
    /// </summary>
    public sealed class OrderMonitor : IHasOrderId, IDisposable
    {
        public Instant SubmissionTime { get; }
        private readonly Request request;
        private readonly ReplaySubject<IHasOrderId> subject = new ReplaySubject<IHasOrderId>();
        public IObservable<IHasOrderId> MessagesObservable => subject.AsObservable();
        public Contract Contract { get; }
        public Order Order { get; }
        public int OrderId { get; }

        //internal OrderMonitor(Request request, Response response, Order order, Contract contract, int orderId)
        internal OrderMonitor(Request request, IObservable<object> response, Order order, Contract contract, int orderId, IClock clock)
        {
            this.request = request;
            response.OfType<IHasOrderId>().Where(m => m.OrderId == orderId).Subscribe(subject);
            Order = order;
            Contract = contract;
            if (orderId < 0)
                throw new ArgumentException(nameof(orderId));
            OrderId = orderId == 0 ? request.GetNextId() : orderId;
            request.PlaceOrder(OrderId, Order, Contract);
            SubmissionTime = clock.GetCurrentInstant();
        }

        public void Cancel() => request.CancelOrder(OrderId); // allow Cancel even if the order has executed or an error has been received.
        public void Dispose() => subject.Dispose();
    }

    /// <summary>
    /// Places an order and returns a object use to monitor the order.
    /// </summary>
    //public OrderMonitor PlaceOrder(Order order, Contract contract, int orderId = 0) =>
    //   new OrderMonitor(Request, Response, order, contract, orderId, Config.Clock);


}

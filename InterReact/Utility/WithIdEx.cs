using System;
using System.Linq;
using System.Reactive.Linq;

namespace InterReact
{
    public static partial class Extensions
    {
        public static IObservable<T> WithRequestId<T>(this IObservable<T> source, int requestId)
            where T: IHasRequestId =>
                source.Where(m => m.RequestId == requestId);

        public static IObservable<T> WithOrderId<T>(this IObservable<T> source, int orderId)
            where T : IHasOrderId =>
                source.Where(m => m.OrderId == orderId);
    }
}

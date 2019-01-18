using InterReact.Enums;
using InterReact.Interfaces;
using InterReact.Messages;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reflection;

namespace InterReact.Extensions
{
    public static class IdEx
    {
        public static IObservable<IHasRequestId> WithRequestId(this IObservable<object> source, int requestId) =>
            source.OfType<IHasRequestId>().Where(x => x.RequestId == requestId);

        public static IObservable<IHasOrderId> WithOrderId(this IObservable<object> source, int orderId) =>
            source.OfType<IHasOrderId>().Where(x => x.OrderId == orderId);
    }
}

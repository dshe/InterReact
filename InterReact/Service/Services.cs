using System;

namespace InterReact
{
    public sealed partial class Services : EditorBrowsableNever
    {
        private readonly Request Request;
        private readonly IObservable<object> Response;

        public Services(Request request, IObservable<object> response)
        {
            Request = request;
            Response = response;
            CurrentTimeObservable = CreateCurrentTimeObservable();
        }
    }
}

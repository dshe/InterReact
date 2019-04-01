using System;
using InterReact.Core;
using InterReact.Interfaces;

namespace InterReact.Service
{
    public sealed partial class Services : EditorBrowsableNever
    {
        private readonly Config Config;
        private readonly Request Request;
        private readonly IObservable<object> Response;

        public Services(Config config, Request request, IObservable<object> response)
        {
            Config = config;
            Response = response;
            Request = request;
        }
    }
}

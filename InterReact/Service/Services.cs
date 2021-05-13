using System;
using Microsoft.Extensions.Logging;

namespace InterReact
{
    public sealed partial class Services : EditorBrowsableNever
    {
        private readonly Config Config;
        private readonly Request Request;
        private readonly IObservable<object> Response;
        private readonly ILogger Logger;

        public Services(Config config, Request request, IObservable<object> response, ILogger logger)
        {
            Config = config;
            Response = response;
            Request = request;
            Logger = logger;
        }
    }
}

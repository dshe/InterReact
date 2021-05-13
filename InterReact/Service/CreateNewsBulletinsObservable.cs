using System;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using InterReact.Extensions;


namespace InterReact
{
    public sealed partial class Services
    {
        /// <summary>
        /// An observable which continually emits news bulletins.
        /// News bulletins inform of important exchange disruptions.
        /// This observable starts with the first subscription and completes when the last observer unsubscribes.
        /// </summary>
        public IObservable<NewsBulletin> CreateNewsBulletinsObservable() =>
            Response
                .ToObservable<NewsBulletin>(
                    () => Request.RequestNewsBulletins(),
                    Request.CancelNewsBulletins)
                .Publish()
                .RefCount();
    }
}

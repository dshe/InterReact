using System;
using System.Reactive.Linq;

namespace InterReact
{
    public sealed partial class Services
    {
        /// <summary>
        /// An observable which continually emits news bulletins.
        /// News bulletins inform of important exchange disruptions.
        /// This observable starts with the first subscription and completes when the last observer unsubscribes.
        /// </summary>
        public IObservable<NewsBulletin> NewsBulletinsObservable { get; }

        private IObservable<NewsBulletin> CreateNewsBulletinsObservable() =>
            Response
                .OfType<NewsBulletin>()
                .ToObservableContinuous(
                    () => Request.RequestNewsBulletins(), Request.CancelNewsBulletins)
                .Publish()
                .RefCount();
    }
}

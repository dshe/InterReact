using System;
using System.Reactive.Linq;

namespace InterReact
{
    public partial class Services
    {
        /// <summary>
        /// An observable which continually emits news bulletins.
        /// This observable supports multiple observers.
        /// </summary>
        public IObservable<NewsBulletin> NewsBulletinsObservable { get; }

        private IObservable<NewsBulletin> CreateNewsBulletinsObservable() =>
            Response
                .OfType<NewsBulletin>()
                .ToObservableContinuous(
                    () => Request.RequestNewsBulletins(all: true), Request.CancelNewsBulletins)
                .Publish()
                .RefCount();
    }
}

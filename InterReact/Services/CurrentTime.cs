namespace InterReact;

public partial class Service
{
    public IObservable<Instant> CreateCurrentTimeObservable()
    {
        IObservable<Instant> observable = Response
            .OfType<CurrentTime>()
            .FirstAsync()
            .Select(currentTime => Instant.FromUnixTimeSeconds(currentTime.Seconds));

        Request.RequestCurrentTime();

        return observable;
    }
}
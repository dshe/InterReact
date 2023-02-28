namespace InterReact;

/*
public static partial class Extension
{
    public static IObservable<object> ThrowIfMessageError(this IObservable<object> source)
    {
        return source.Do(message =>
        {
            if (message is not MessageError)
                return;
            throw new InvalidOperationException();
        });
    }
}

internal sealed class MessageError
{
    public Instant Time { get; } = SystemClock.Instance.GetCurrentInstant();
    public string Code { get; }
    public string Message { get; }
    public Exception Exception { get; }
    internal MessageError(string code, Exception exception, string message)
    {
        Code = code;
        Exception = exception;
        Message = message;
    }
}
*/

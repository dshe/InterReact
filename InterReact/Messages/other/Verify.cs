namespace InterReact;

[Message]
public sealed record VerifyMessageApi
{
    public string Data { get; }
    internal VerifyMessageApi(ResponseReader r)
    {
        r.IgnoreMessageVersion();
        Data = r.ReadString();
    }
}

[Message]
public sealed record VerifyCompleted
{
    public bool IsSuccessful { get; }
    public string ErrorText { get; }
    internal VerifyCompleted(ResponseReader r)
    {
        r.IgnoreMessageVersion();
        IsSuccessful = r.ReadBool();
        ErrorText = r.ReadString();
    }
}

[Message]
public sealed record VerifyAndAuthorizeMessageApi
{
    public string ApiData { get; }
    public string XyzChallenge { get; }
    internal VerifyAndAuthorizeMessageApi(ResponseReader r)
    {
        r.IgnoreMessageVersion();
        ApiData = r.ReadString();
        XyzChallenge = r.ReadString();
    }
}

[Message]
public sealed record VerifyAndAuthorizeCompleted
{
    public bool IsSuccessful { get; }
    public string ErrorText { get; }

    internal VerifyAndAuthorizeCompleted(ResponseReader r)
    {
        r.IgnoreMessageVersion();
        IsSuccessful = r.ReadBool();
        ErrorText = r.ReadString();
    }
}

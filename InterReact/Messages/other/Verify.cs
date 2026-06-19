namespace InterReact;

[Message]
public sealed record VerifyMessageApi
{
    public string Data { get; init; } = "";
    internal VerifyMessageApi() { }
    internal VerifyMessageApi(ResponseReader r)
    {
        r.IgnoreMessageVersion();
        Data = r.ReadString();
    }
}

[Message]
public sealed record VerifyCompleted
{
    public bool IsSuccessful { get; init; }
    public string ErrorText { get; init; } = "";
    internal VerifyCompleted() { }
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
    public string ApiData { get; init; } = "";
    public string XyzChallenge { get; init; } = "";
    internal VerifyAndAuthorizeMessageApi() { }
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
    public bool IsSuccessful { get; init; }
    public string ErrorText { get; init; } = "";
    internal VerifyAndAuthorizeCompleted() { }
    internal VerifyAndAuthorizeCompleted(ResponseReader r)
    {
        r.IgnoreMessageVersion();
        IsSuccessful = r.ReadBool();
        ErrorText = r.ReadString();
    }
}

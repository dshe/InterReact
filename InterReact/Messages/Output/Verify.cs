namespace InterReact;

public sealed class VerifyMessageApi
{
    public string Data { get; } = "";
    internal VerifyMessageApi(ResponseReader r)
    {
        r.IgnoreMessageVersion();
        Data = r.ReadString();
    }
}

public sealed class VerifyCompleted
{
    public bool IsSuccessful { get; }
    public string ErrorText { get; } = "";
    internal VerifyCompleted(ResponseReader r)
    {
        r.IgnoreMessageVersion();
        IsSuccessful = r.ReadBool();
        ErrorText = r.ReadString();
    }
}

public sealed class VerifyAndAuthorizeMessageApi
{
    public string ApiData { get; } = "";
    public string XyzChallenge { get; } = "";
    internal VerifyAndAuthorizeMessageApi(ResponseReader r)
    {
        r.IgnoreMessageVersion();
        ApiData = r.ReadString();
        XyzChallenge = r.ReadString();
    }
}

public sealed class VerifyAndAuthorizeCompleted
{
    public bool IsSuccessful { get; }
    public string ErrorText { get; } = "";
    
    internal VerifyAndAuthorizeCompleted(ResponseReader r)
    {
        r.IgnoreMessageVersion();
        IsSuccessful = r.ReadBool();
        ErrorText = r.ReadString();
    }
}

namespace InterReact.Messages
{
    public sealed class VerifyMessageApi
    {
        public string Data { get; internal set; }
    }

    public sealed class VerifyCompleted
    {
        public bool IsSuccessful { get; internal set; }
        public string ErrorText { get; internal set; }
    }

    public sealed class VerifyAndAuthorizeMessageApi
    {
        public string ApiData { get; internal set; }
        public string XyzChallenge { get; internal set; }
    }

    public sealed class VerifyAndAuthorizeCompleted
    {
        public bool IsSuccessful { get; internal set; }
        public string ErrorText { get; internal set; }
    }

}

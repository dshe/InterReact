namespace InterReact
{
    public sealed class VerifyMessageApi
    {
        public string Data { get; }
        internal VerifyMessageApi(ResponseReader c)
        {
            c.IgnoreVersion();
            Data = c.ReadString();
        }
    }

    public sealed class VerifyCompleted
    {
        public bool IsSuccessful { get; }
        public string ErrorText { get; }
        internal VerifyCompleted(ResponseReader c)
        {
            c.IgnoreVersion();
            IsSuccessful = c.ReadBool();
            ErrorText = c.ReadString();
        }
    }

    public sealed class VerifyAndAuthorizeMessageApi
    {
        public string ApiData { get; }
        public string XyzChallenge { get; }
        internal VerifyAndAuthorizeMessageApi(ResponseReader c)
        {
            c.IgnoreVersion();
            ApiData = c.ReadString();
            XyzChallenge = c.ReadString();
        }
    }

    public sealed class VerifyAndAuthorizeCompleted
    {
        public bool IsSuccessful { get; }
        public string ErrorText { get; }
        internal VerifyAndAuthorizeCompleted(ResponseReader c)
        {
            c.IgnoreVersion();
            IsSuccessful = c.ReadBool();
            ErrorText = c.ReadString();
        }
    }
}

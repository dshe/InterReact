namespace InterReact;

[Message]
public sealed record ManagedAccounts
{
    public string Accounts { get; } = "";
    internal ManagedAccounts() { }
    internal ManagedAccounts(ResponseReader r)
    {
        r.IgnoreMessageVersion();
        Accounts = r.ReadString();
    }
}

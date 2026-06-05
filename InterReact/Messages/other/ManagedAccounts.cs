namespace InterReact;

[Message]
public sealed record ManagedAccounts
{
    public IReadOnlyList<string> Accounts { get; }
    internal ManagedAccounts() => Accounts = [];
    internal ManagedAccounts(ResponseReader r)
    {
        r.IgnoreMessageVersion();
        Accounts = r.ReadString().Split(',', StringSplitOptions.RemoveEmptyEntries);
        r.Options.ManagedAccounts = Accounts;
    }
}

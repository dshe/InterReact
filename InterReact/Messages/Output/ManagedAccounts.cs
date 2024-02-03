namespace InterReact;

public sealed class ManagedAccounts
{
    public IEnumerable<string> Accounts { get; } = Enumerable.Empty<string>();

    internal ManagedAccounts(ResponseReader r)
    {
        r.IgnoreMessageVersion();
        Accounts = r
            .ReadString()
            .Split(',', StringSplitOptions.RemoveEmptyEntries);
        
    }
}

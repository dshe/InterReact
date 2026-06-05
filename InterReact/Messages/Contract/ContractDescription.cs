namespace InterReact;

[Message]
public sealed record ContractDescription
{
    public Contract Contract { get; init;  } = new Contract();
    public IList<string> DerivativeSecTypes { get; init; }
    internal ContractDescription() => DerivativeSecTypes = [];
    internal ContractDescription(ResponseReader r)
    {
        Contract.ContractId = r.ReadInt();
        Contract.Symbol = r.ReadString();
        Contract.SecurityType = r.ReadString();
        Contract.PrimaryExchange = r.ReadString();
        Contract.Currency = r.ReadString();
        DerivativeSecTypes = r.GetStringList();
        Contract.Description = r.ReadString();
        Contract.IssuerId = r.ReadString();
    }
}

namespace InterReact;

public sealed class ContractDescription
{
    public Contract Contract { get; } = new Contract();
    public IList<string> DerivativeSecTypes { get; }
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

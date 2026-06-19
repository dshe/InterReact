namespace InterReact;

[Message]
public sealed record FamilyCodes
{
    public IList<FamilyCode> Codes { get; } = [];
    internal FamilyCodes() => Codes = [];
    internal FamilyCodes(ResponseReader r)
    {
        int n = r.ReadInt();
        for (int i = 0; i < n; i++)
            Codes.Add(new FamilyCode(r));
    }
}

public sealed record FamilyCode
{
    public string AccountId { get; init; } = "";
    public string FamilyCodeStr { get; init; } = "";
    internal FamilyCode() { }
    internal FamilyCode(ResponseReader r)
    {
        AccountId = r.ReadString();
        FamilyCodeStr = r.ReadString();
    }
}

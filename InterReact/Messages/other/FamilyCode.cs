namespace InterReact;

[Message]
public sealed record FamilyCodes
{
    public IList<FamilyCode> Codes { get; }
    internal FamilyCodes() => Codes = [];
    internal FamilyCodes(ResponseReader r)
    {
        int n = r.ReadInt();
        Codes = new List<FamilyCode>(n);
        for (int i = 0; i < n; i++)
            Codes.Add(new FamilyCode(r));
    }
}

[Message]
public sealed record FamilyCode
{
    public string AccountId { get; }
    public string FamilyCodeStr { get; }
    internal FamilyCode(ResponseReader r)
    {
        AccountId = r.ReadString();
        FamilyCodeStr = r.ReadString();
    }
}

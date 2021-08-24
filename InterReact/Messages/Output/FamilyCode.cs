using System.Collections.Generic;

namespace InterReact
{
    public sealed class FamilyCodes
    {
        public List<FamilyCode> Codes { get; } = new();
        internal FamilyCodes() { }
        internal FamilyCodes(ResponseReader r)
        {
            int n = r.ReadInt();
            for (int i = 0; i < n; i++)
                Codes.Add(new FamilyCode(r));
        }
    }

    public sealed class FamilyCode
    {
        public string AccountId { get; } = "";
        public string FamilyCodeStr { get; } = "";
        internal FamilyCode() { }
        internal FamilyCode(ResponseReader r)
        {
            AccountId = r.ReadString();
            FamilyCodeStr = r.ReadString();
        }
    }
}

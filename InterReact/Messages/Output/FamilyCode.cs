using System.Collections.Generic;

namespace InterReact
{
    public sealed class FamilyCodes // output
    {
        public List<FamilyCode> Codes { get; } = new List<FamilyCode>();
        internal FamilyCodes(ResponseReader r)
        {
            int n = r.ReadInt();
            for (int i = 0; i < n; i++)
                Codes.Add(new FamilyCode(r));
        }
    }

    public sealed class FamilyCode // output
    {
        public string AccountId { get; }
        public string FamilyCodeStr { get; }
        internal FamilyCode(ResponseReader r)
        {
            AccountId = r.ReadString();
            FamilyCodeStr = r.ReadString();
        }
    }
}

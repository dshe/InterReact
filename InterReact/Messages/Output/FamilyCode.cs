using System.Collections.Generic;

namespace InterReact
{
    public sealed class FamilyCodes // output
    {
        public List<FamilyCode> Codes { get; } = new List<FamilyCode>();
        internal FamilyCodes(ResponseReader c)
        {
            int n = c.ReadInt();
            for (int i = 0; i < n; i++)
                Codes.Add(new FamilyCode(c));
        }
    }

    public sealed class FamilyCode // output
    {
        public string AccountId { get; }
        public string FamilyCodeStr { get; }
        internal FamilyCode(ResponseReader c)
        {
            AccountId = c.ReadString();
            FamilyCodeStr = c.ReadString();
        }
    }
}

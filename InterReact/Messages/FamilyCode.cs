using InterReact.Core;
using System.Collections.Generic;
using System.Linq;

namespace InterReact.Messages
{
    public sealed class FamilyCodes // output
    {
        public IList<FamilyCode> Codes = new List<FamilyCode>();
        internal FamilyCodes(ResponseComposer c)
        {
            var n = c.ReadInt();
            for (int i = 0; i < n; i++)
                Codes.Add(new FamilyCode(c));
        }
    }

    public sealed class FamilyCode // output
    {
        public string AccountId { get; }
        public string FamilyCodeStr { get; }
        internal FamilyCode(ResponseComposer c)
        {
            AccountId = c.ReadString();
            FamilyCodeStr = c.ReadString();
        }
    }
}

using InterReact.Core;
using System.Collections.Generic;
using System.Linq;

namespace InterReact.Messages
{
    public sealed class FamilyCode // output
    {
        public string AccountId { get; }

        public string FamilyCodeStr { get; }

        internal FamilyCode(ResponseReader c)
        {
            AccountId = c.ReadString();
            FamilyCodeStr = c.ReadString();
        }

        internal static IReadOnlyList<FamilyCode> GetAll(ResponseReader c)
        {
            var n = c.Read<int>();
            return Enumerable.Repeat(new FamilyCode(c), n).ToList();
        }

    }

}

﻿using System.Collections.Generic;

namespace InterReact;

public sealed class ContractDescription // output
{
    public Contract Contract { get; } = new();
    public IList<string> DerivativeSecTypes { get; } = new List<string>();
 
    internal ContractDescription() { }
    internal ContractDescription(ResponseReader r)
    {
        Contract.ContractId = r.ReadInt();
        Contract.Symbol = r.ReadString();
        Contract.SecurityType = r.ReadStringEnum<SecurityType>();
        Contract.PrimaryExchange = r.ReadString();
        Contract.Currency = r.ReadString();
        r.AddStringsToList(DerivativeSecTypes);
        if (r.Connector.SupportsServerVersion(ServerVersion.MIN_SERVER_VER_BOND_ISSUERID))
        {
            Contract.Description = r.ReadString();
            Contract.IssuerId = r.ReadString();    
        }
    }
}

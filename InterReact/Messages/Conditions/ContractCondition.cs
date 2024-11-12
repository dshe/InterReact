using System.Diagnostics.CodeAnalysis;
namespace InterReact;

[SuppressMessage("Usage", "CA1012", Scope = "member")]
[SuppressMessage("Usage", "CA1307", Scope = "member")]
[SuppressMessage("Usage", "CA1309", Scope = "member")]
[SuppressMessage("Usage", "CA1310", Scope = "member")]
[SuppressMessage("Usage", "CA1031", Scope = "member")]
[SuppressMessage("Usage", "IDE0057", Scope = "member")]
[SuppressMessage("Usage", "IDE0018", Scope = "member")]
public abstract class ContractCondition : OperatorCondition
{
    public int ConId { get; set; }
    public string Exchange { get; set; } = "";

    const string delimiter = " of ";

    public Func<int, string, string> ContractResolver { get; set; }

    public ContractCondition()
    {
        ContractResolver = (conid, exch) => conid + "(" + exch + ")";
    }

    public override string ToString()
    {
        return Type + delimiter + ContractResolver(ConId, Exchange) + base.ToString();
    }

    [SuppressMessage("Usage", "IDE0019", Scope = "member")]
    public override bool Equals(object? obj)
    {
        var other = obj as ContractCondition;

        if (other == null)
            return false;

        return base.Equals(obj)
            && ConId == other.ConId
            && Exchange.Equals(other.Exchange);
    }

    public override int GetHashCode()
    {
        return base.GetHashCode() + ConId.GetHashCode() + Exchange.GetHashCode();
    }

    protected override bool TryParse(string cond)
    {
        ArgumentNullException.ThrowIfNull(cond);

        try
        {
            if (cond.Substring(0, cond.IndexOf(delimiter)) != Type.ToString())
                return false;

            cond = cond.Substring(cond.IndexOf(delimiter) + delimiter.Length);
            int conid;

            if (!int.TryParse(cond[..cond.IndexOf('(')], out conid))
                return false;

            ConId = conid;
            cond = cond[(cond.IndexOf('(') + 1)..];
            Exchange = cond[..cond.IndexOf(')')];
            cond = cond[(cond.IndexOf(')') + 1)..];

            return base.TryParse(cond);
        }
        catch
        {
            return false;
        }
    }

    public override void Deserialize(ResponseReader r)
    {
        ArgumentNullException.ThrowIfNull(r);
        base.Deserialize(r);
        ConId = r.ReadInt();
        Exchange = r.ReadString();
    }

    public override void Serialize(RequestMessage message)
    {
        ArgumentNullException.ThrowIfNull(message);
        base.Serialize(message);
        message.Write(ConId);
        message.Write(Exchange);
    }
}

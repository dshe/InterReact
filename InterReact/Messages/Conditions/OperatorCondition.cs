using System.Diagnostics.CodeAnalysis;
namespace InterReact;

[SuppressMessage("Usage", "CA1307", Scope = "member")]
[SuppressMessage("Usage", "CA1309", Scope = "member")]
[SuppressMessage("Usage", "CA1310", Scope = "member")]
[SuppressMessage("Usage", "CA1062", Scope = "member")]
[SuppressMessage("Usage", "IDE0019", Scope = "member")]
public abstract class OperatorCondition : OrderCondition
{
    protected abstract string Value { get; set; }
    public bool IsMore { get; set; }

    const string header = " is ";

    public override string ToString()
    {
        return header + (IsMore ? ">= " : "<= ") + Value;
    }

    public override bool Equals(object? obj)
    {
        var other = obj as OperatorCondition;

        if (other == null)
            return false;

        return base.Equals(obj)
            && Value.Equals(other.Value)
            && IsMore == other.IsMore;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode() + Value.GetHashCode() + IsMore.GetHashCode();
    }

    protected override bool TryParse(string cond)
    {
        if (!cond.StartsWith(header))
            return false;

        try
        {
            cond = cond.Replace(header, "");

            if (!cond.StartsWith(">=") && !cond.StartsWith("<="))
                return false;

            IsMore = cond.StartsWith(">=");

            if (base.TryParse(cond[cond.LastIndexOf(' ')..]))
                cond = cond[..cond.LastIndexOf(' ')];

            Value = cond[3..];
        }
        catch
        {
            return false;
        }

        return true;
    }

    public override void Deserialize(ResponseReader r)
    {
        base.Deserialize(r);

        IsMore = r.ReadBool();
        Value = r.ReadString();
    }

    public override void Serialize(RequestMessage message)
    {
        base.Serialize(message);
        message.Write(IsMore);
        message.Write(Value);
    }
}

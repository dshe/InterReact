namespace InterReact;

/// <summary>
/// Missing Values are indicated with null.
/// Missing Values are indicated with double.MaxValue.
/// </summary>
[Message]
public sealed record OptionComputationTick : TickBase
{
    public int TickAttrib { get; init; }
    public double ImpliedVolatility { get; init; }
    public double Delta { get; init; }
    public double OptPrice { get; init; } = double.MaxValue;
    public double PvDividend { get; init; } = double.MaxValue;
    public double Gamma { get; init; } = double.MaxValue;
    public double Vega { get; init; } = double.MaxValue;
    public double Theta { get; init; } = double.MaxValue;
    public double UndPrice { get; init; } = double.MaxValue;
    internal OptionComputationTick() { }
    internal OptionComputationTick(ResponseReader r)
    {
        RequestId = r.ReadInt();
        TickType = r.ReadEnum<TickType>();
        TickAttrib = r.ReadInt();

        ImpliedVolatility = r.ReadDouble();
        if (ImpliedVolatility == -1)
            ImpliedVolatility = double.MaxValue;

        Delta = r.ReadDouble();
        if (Delta == -2)
            Delta = double.MaxValue;

        if (TickType == TickType.ModelOptionComputation || TickType == TickType.DelayedAskOptionComputation)
        {
            OptPrice = r.ReadDouble();
            if (OptPrice == -1)
                OptPrice = double.MaxValue;

            PvDividend = r.ReadDouble();
            if (PvDividend == -1)
                PvDividend = double.MaxValue;
        }

        Gamma = r.ReadDouble();
        if (Gamma == -2)
            Gamma = double.MaxValue;

        Vega = r.ReadDouble();
        if (Vega == -2)
            Vega = double.MaxValue;

        Theta = r.ReadDouble();
        if (Theta == -2)
            Theta = double.MaxValue;

        UndPrice = r.ReadDouble();
        if (UndPrice == -1)
            UndPrice = double.MaxValue;
    }
}

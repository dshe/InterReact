namespace InterReact;

/// <summary>
/// Missing Values are indicated with null.
/// IB's Java client indicates missing Values using DOUBLE_MAX.
/// </summary>
public sealed class OptionComputationTick : ITick
{
    public int RequestId { get; }
    public TickType TickType { get; } = TickType.Undefined;
    public int TickAttrib { get; }
    public double ImpliedVolatility { get; }
    public double Delta { get; }
    public double OptPrice { get; } = double.MaxValue;
    public double PvDividend { get; } = double.MaxValue;
    public double Gamma { get; } = double.MaxValue;
    public double Vega { get; } = double.MaxValue;
    public double Theta { get; } = double.MaxValue;
    public double UndPrice { get; } = double.MaxValue;
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

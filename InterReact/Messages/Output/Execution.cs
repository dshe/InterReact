using System.Collections.Generic;

namespace InterReact;

public sealed class Execution : IHasRequestId, IHasOrderId, IHasExecutionId
{
    // Store executions by executionId so that they can be associated with CommissionReport (above).
    // This assumes that CommissionReport always follows Execution.
    internal static Dictionary<string, Execution> Executions = new();

    /// <summary>
    /// RequestId will be -1 if this object sent due to an execution rather than a request for executions.
    /// </summary>
    public int RequestId { get; }

    public int OrderId { get; }

    /// <summary>
    /// Unique order execution id.
    /// </summary>
    public string ExecutionId { get; } = "";

    /// <summary>
    /// The Id of the client which placed the order.
    /// Note: TWS orders have a fixed client id of "0."
    /// </summary>
    public int ClientId { get; }

    /// <summary>
    /// The order execution time.
    /// </summary>
    public string Time { get; } = "";

    /// <summary>
    /// Cusomer account number.
    /// </summary>
    public string Account { get; } = "";

    public string Exchange { get; } = "";
    public ExecutionSide Side { get; } = ExecutionSide.Undefined;

    /// <summary>
    /// The number of shares filled.
    /// </summary>
    public double Shares { get; }

    /// <summary>
    /// The order execution price, not including commissions.
    /// </summary>
    public double Price { get; }

    /// <summary>
    /// The TWS id used to identify orders, which remains the same over TWS sessions.
    /// </summary>
    public int PermanentId { get; }

    /// <summary>
    /// Identifies the position as one to be liquidated last should the need arise.
    /// </summary>
    public int Liquidation { get; }
    public double CumulativeQuantity { get; }

    /// <summary>
    /// The average price, which includes commissions.
    /// </summary>
    public double AveragePrice { get; }
    public string OrderReference { get; } = "";

    /// <summary>
    /// IncludeAll the Economic Value Rule name and the respective optional argument. The two Values should be separated by a colon. For example, aussieBond:YearsToExpiration=3. When the optional argument is not present, the first value will be followed by a colon.
    /// </summary>
    public string EconomicValueRule { get; } = "";

    /// <summary>
    /// Tells you approximately how much the market value of a contract would change if the price were to change by 1. It cannot be used to get market value by multiplying the price by the approximate multiplier.
    /// </summary>
    public double EconomicValueMultiplier { get; }

    public string ModelCode { get; } = "";

    public Liquidity LastLiquidity { get; } = Liquidity.None;

    public Contract Contract { get; } = new();

    internal Execution() { }

    internal Execution(ResponseReader r)
    {
        if (r.Connector.ServerVersionCurrent < ServerVersion.LAST_LIQUIDITY)
            r.RequireVersion(10);
        RequestId = r.ReadInt();
        OrderId = r.ReadInt();
        Contract.ContractId = r.ReadInt();
        Contract.Symbol = r.ReadString();
        Contract.SecurityType = r.ReadStringEnum<SecurityType>();
        Contract.LastTradeDateOrContractMonth = r.ReadString();
        Contract.Strike = r.ReadDouble();
        Contract.Right = r.ReadStringEnum<OptionRightType>();
        Contract.Multiplier = r.ReadString();
        Contract.Exchange = r.ReadString();
        Contract.Currency = r.ReadString();
        Contract.LocalSymbol = r.ReadString();
        Contract.TradingClass = r.ReadString();
        ExecutionId = r.ReadString();
        Time = r.ReadString();
        Account = r.ReadString();
        Exchange = r.ReadString();
        Side = r.ReadStringEnum<ExecutionSide>();
        Shares = r.ReadDouble();
        Price = r.ReadDouble();
        PermanentId = r.ReadInt();
        ClientId = r.ReadInt();
        Liquidation = r.ReadInt();
        CumulativeQuantity = r.ReadDouble();
        AveragePrice = r.ReadDouble();
        OrderReference = r.ReadString();
        EconomicValueRule = r.ReadString();
        EconomicValueMultiplier = r.ReadDouble();
        if (r.Connector.SupportsServerVersion(ServerVersion.MODELS_SUPPORT))
            ModelCode = r.ReadString();
        if (r.Connector.SupportsServerVersion(ServerVersion.LAST_LIQUIDITY))
            LastLiquidity = r.ReadEnum<Liquidity>();
        Executions[ExecutionId] = this;
    }
}

public sealed class ExecutionEnd : IHasRequestId
{
    public int RequestId { get; }
    internal ExecutionEnd(ResponseReader r)
    {
        r.IgnoreVersion();
        RequestId = r.ReadInt();
    }
}

namespace InterReact;

public sealed class WshEventData // input
{
    public int ContractId { get; init; } = int.MaxValue;
    public string Filter { get; init; } = "";
    public bool FillWatchlist { get; init; }
    public bool FillPortfolio { get; init; }
    public bool FillCompetitors { get; init; }
    public string StartDate { get; init; } = "";
    public string EndDate { get; init; } = "";
    public int TotalLimit { get; init; } = int.MaxValue;
}

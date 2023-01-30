namespace InterReact;

public sealed class WshEventDataIn
{
    public int ContractId { get; init; }

    public string Filter { get; init; } = "";

    public bool FillWatchlist { get; init; }

    public bool FillPortfolio { get; init; }

    public bool FillCompetitors { get; init; }

    public string StartDate { get; init; } = "";

    public string EndDate { get; init; } = "";

    public int TotalLimit { get; init; }
}

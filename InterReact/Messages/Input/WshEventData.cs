namespace InterReact;

public sealed class WshEventData // input
{
    public int ContractId { get; set; } = int.MaxValue;
    public string Filter { get; set; } = "";
    public bool FillWatchlist { get; set; }
    public bool FillPortfolio { get; set; }
    public bool FillCompetitors { get; set; }
    public string StartDate { get; set; } = "";
    public string EndDate { get; set; } = "";
    public int TotalLimit { get; set; } = int.MaxValue;
}

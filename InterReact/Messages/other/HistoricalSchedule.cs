namespace InterReact;

[Message]
public sealed record HistoricalSchedule : IHasRequestId
{
    public int RequestId { get; }
    public string StartDate { get; } = "";
    public string EndDate { get; } = "";
    public string Timezone { get; } = "";
    public IList<HistoricalSession> Sessions { get; } = [];
    internal HistoricalSchedule() { }
    internal HistoricalSchedule(ResponseReader r)
    {
        RequestId = r.ReadInt();
        StartDate = r.ReadString();
        EndDate = r.ReadString();
        Timezone = r.ReadString();
        int n = r.ReadInt();
        for (int i = 0; i < n; i++)
            Sessions.Add(new HistoricalSession(r));
    }
}

[Message]
public sealed record HistoricalSession
{
    public string SessionStartTime { get; init; } = "";
    public string SessionEndTime { get; init; } = "";
    public string SessionRefDate { get; init; } = "";
    internal HistoricalSession() { }
    internal HistoricalSession(ResponseReader r)
    {
        SessionStartTime = r.ReadString();
        SessionEndTime = r.ReadString();
        SessionRefDate = r.ReadString();
    }
}

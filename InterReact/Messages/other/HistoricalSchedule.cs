namespace InterReact;

[Message]
public sealed record HistoricalSchedule : IHasRequestId
{
    public int RequestId { get; }
    public string StartDate { get; } = "";
    public string EndDate { get; } = "";
    public string Timezone { get; } = "";
    public IList<HistoricalSession> Sessions { get; }
    internal HistoricalSchedule() => Sessions = [];
    internal HistoricalSchedule(ResponseReader r)
    {
        RequestId = r.ReadInt();
        StartDate = r.ReadString();
        EndDate = r.ReadString();
        Timezone = r.ReadString();
        int n = r.ReadInt();
        Sessions = new List<HistoricalSession>(n);
        for (int i = 0; i < n; i++)
            Sessions.Add(new HistoricalSession(r));
    }
}

[Message]
public sealed record HistoricalSession
{
    public string SessionStartTime { get; }
    public string SessionEndTime { get; }
    public string SessionRefDate { get; }
    internal HistoricalSession(ResponseReader r)
    {
        SessionStartTime = r.ReadString();
        SessionEndTime = r.ReadString();
        SessionRefDate = r.ReadString();
    }
}

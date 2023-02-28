namespace InterReact;

public sealed class HistoricalSchedule : IHasRequestId
{
    public int RequestId { get; }
    public string StartDate { get; }
    public string EndDate { get; }
    public string Timezone { get; }
    IList<HistoricalSession> Sessions { get; } = new List<HistoricalSession>();
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

public sealed class HistoricalSession
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

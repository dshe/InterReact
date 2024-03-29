﻿namespace InterReact;

public sealed class WshMetaData : IHasRequestId
{
    public int RequestId { get; }
    public string Data { get; }
    internal WshMetaData(ResponseReader reader)
    {
        RequestId = reader.ReadInt();
        Data = reader.ReadString();
    }
}

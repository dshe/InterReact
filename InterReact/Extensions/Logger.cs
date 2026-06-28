namespace InterReact;

// This C# 14 extension method syntax is not currently compatible with LoggerMessage source generation.

public static partial class Xtensions
{
    [LoggerMessage(EventId = 2, EventName = "ResponseMessage", Level = LogLevel.Debug, Message = "{Message}.")]
    internal static partial void LogResponseMessage(this ILogger logger, string message);

    [LoggerMessage(EventId = 1, EventName = "ResponseString", Level = LogLevel.Trace, Message = "{CallerInfo}\r\n\t\"{Input}\" => {Output} ({TypeName}).")]
    internal static partial void LogResponseString(this ILogger logger, string callerInfo, string input, string output, string typeName);
}

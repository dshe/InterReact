namespace InterReact;

public static partial class Extension
{
    [LoggerMessage(EventId = 1, EventName = "ResponseMessage", Level = LogLevel.Debug, Message = "Response received: {Message}.")]
    internal static partial void LogResponseMessage(this ILogger logger, string message);

    [LoggerMessage(EventId = 2, EventName = "ResponseString", Level = LogLevel.Debug, Message = "Response string received: {CallerInfo}\r\n\t\"{Input}\" => {Output} ({TypeName}).")]
    internal static partial void LogResponseString(this ILogger logger, string callerInfo, string input, string output, string typeName);
}


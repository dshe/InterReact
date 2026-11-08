namespace InterReact;

// This C# 14 extension method syntax is not currently compatible with LoggerMessage source generation.

public static partial class Xtensions
{
    [LoggerMessage(EventId = 1, EventName = "RequestMessage", Level = LogLevel.Debug, Message = "{MemberName}({Strings}]).")]
    internal static partial void LogRequestMessage(this ILogger logger, string memberName, string strings);

    [LoggerMessage(EventId = 2, EventName = "ResponseMessage", Level = LogLevel.Debug, Message = "{Message}.")]
    internal static partial void LogResponseMessage(this ILogger logger, string message);

}

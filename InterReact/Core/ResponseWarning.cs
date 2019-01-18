using System;

namespace InterReact.Core
{
    /// <summary>
    /// This warning message is emitted when:
    ///  - messages are shorter than expected
    ///  - new enum codes(numeric) are received
    ///  - new stringEnum codes(strings) are received
    /// </summary>
    public sealed class ResponseWarning
    {
        public string StackTrace { get; } = Environment.StackTrace;
        public string Message { get; }
        internal ResponseWarning(string message) => Message = message;
    }
}

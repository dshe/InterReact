using System;
using InterReact.Enums;
using InterReact.StringEnums;
using NodaTime;

namespace InterReact.Messages
{
    public sealed class ExecutionFilter // input
    {
        /// <summary>
        /// This is only relevant for Financial Advisor (FA) accounts.
        /// </summary>
        public string Account { get; set; } = "";

        /// <summary>
        /// Zero means no filtering on this field.
        /// </summary>
        public int ClientId { get; set; }

        public LocalDateTime Time { get; set; }

        public string Symbol { get; set; } = "";

        public SecurityType SecurityType { get; set; } = SecurityType.Undefined;

        public string Exchange { get; set; } = "";

        public string Side { get; set; } = "";
    }

}

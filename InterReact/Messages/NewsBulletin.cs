using System;
using InterReact.Enums;
using InterReact.StringEnums;

namespace InterReact.Messages
{
    public sealed class NewsBulletin
    {
        public int MessageId { get; internal set; }

        public NewsBulletinType Type { get; internal set; } = NewsBulletinType.Undefined;

        public string Message { get; internal set; }

        /// <summary>
        /// The exchange from which this message originated.
        /// </summary>
        public string Origin { get; internal set; }
    }

}
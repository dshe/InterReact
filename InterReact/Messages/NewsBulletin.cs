using InterReact.Core;
using InterReact.Enums;

namespace InterReact.Messages
{
    public sealed class NewsBulletin
    {
        public int MessageId { get; }

        public NewsBulletinType Type { get; }

        public string Message { get; }

        /// <summary>
        /// The exchange from which this message originated.
        /// </summary>
        public string Origin { get; }
        internal NewsBulletin(ResponseComposer c)
        {
            c.IgnoreVersion();
            MessageId = c.ReadInt();
            Type = c.ReadEnum<NewsBulletinType>();
            Message = c.ReadString();
            Origin = c.ReadString();
        }
    }
}
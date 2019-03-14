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
        internal NewsBulletin(ResponseReader c)
        {
            c.IgnoreVersion();
            MessageId = c.Read<int>();
            Type = c.Read<NewsBulletinType>();
            Message = c.ReadString();
            Origin = c.ReadString();
        }
    }
}
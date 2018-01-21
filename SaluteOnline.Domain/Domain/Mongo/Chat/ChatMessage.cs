using System;
using System.Collections.Generic;

namespace SaluteOnline.Domain.Domain.Mongo.Chat
{
    public class ChatMessage
    {
        public string Message { get; set; }
        public DateTimeOffset Sent { get; set; }
        public ChatMember Sender { get; set; }
        public List<ChatSeenStamp> Seen { get; set; } = new List<ChatSeenStamp>();
    }
}

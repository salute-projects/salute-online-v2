using System;
using System.Collections.Generic;

namespace SaluteOnline.ChatService.Domain
{
    public class ChatMessage
    {
        public string Message { get; set; }
        public DateTimeOffset Sent { get; set; }
        public Guid Sender { get; set; }
        public List<ChatSeenStamp> Seen { get; set; } = new List<ChatSeenStamp>();
    }
}

using System;

namespace SaluteOnline.ChatService.Domain
{
    public class ChatSeenStamp
    {
        public Guid Observer { get; set; }
        public DateTimeOffset TimeStamp { get; set; }
    }
}

using System;

namespace SaluteOnline.Domain.Domain.Mongo.Chat
{
    public class ChatSeenStamp
    {
        public int ObserverId { get; set; }
        public DateTimeOffset TimeStamp { get; set; }
    }
}

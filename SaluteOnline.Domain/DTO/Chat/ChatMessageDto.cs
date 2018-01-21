using System;

namespace SaluteOnline.Domain.DTO.Chat
{
    public class ChatMessageDto
    {
        public Guid Guid { get; set; }
        public string Message { get; set; }
        public DateTimeOffset Sent { get; set; }
        public int SenderId { get; set; }
        public EntityType SenderType { get; set; }
        public bool Seen { get; set; }
        public bool My { get; set; }
        public string Avatar { get; set; }
    }
}

using System;
using SaluteOnline.Shared.Common;

namespace SaluteOnline.ChatService.Domain.DTO
{
    public class ChatMessageDto
    {
        public Guid Guid { get; set; }
        public string Message { get; set; }
        public DateTimeOffset Sent { get; set; }
        public Guid Sender { get; set; }
        public EntityType SenderType { get; set; }
        public bool Seen { get; set; }
        public bool My { get; set; }
    }
}

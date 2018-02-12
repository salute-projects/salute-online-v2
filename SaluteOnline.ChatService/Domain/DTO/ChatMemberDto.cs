using System;
using SaluteOnline.Domain.DTO;

namespace SaluteOnline.ChatService.Domain.DTO
{
    public class ChatMemberDto
    {
        public Guid SubjectId { get; set; }
        public EntityType Type { get; set; }
        public string Title { get; set; }
    }
}

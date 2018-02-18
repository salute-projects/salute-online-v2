using System;
using SaluteOnline.Shared.Common;

namespace SaluteOnline.ChatService.Domain.DTO
{
    public class ChatMemberDto
    {
        public Guid SubjectId { get; set; }
        public EntityType Type { get; set; }
        public string Title { get; set; }
    }
}

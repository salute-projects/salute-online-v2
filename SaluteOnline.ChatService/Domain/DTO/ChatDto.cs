using System;
using System.Collections.Generic;

namespace SaluteOnline.ChatService.Domain.DTO
{
    public class ChatDto
    {
        public Guid Guid { get; set; }

        public DateTimeOffset Created { get; set; }
        public DateTimeOffset LastUpdated { get; set; }

        public List<ChatMemberDto> Participants { get; set; } = new List<ChatMemberDto>();

        public bool IsPrivate { get; set; }
        public string Title { get; set; }
        public string Avatar { get; set; }

        public int NewMessages { get; set; }
    }
}

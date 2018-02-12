using System;
using SaluteOnline.Domain.DTO;

namespace SaluteOnline.ChatService.Domain.DTO
{
    public class PostPrivateMessageDto
    {
        public Guid? ChatGuid { get; set; }

        public string SenderGuid { get; set; }
        public EntityType SenderType { get; set; }
        public string SenderTitle { get; set; }

        public string ReceiverGuid { get; set; }
        public EntityType ReceiverType { get; set; }
        public string ReceiverTitle { get; set; }

        public string Message { get; set; }
    }
}

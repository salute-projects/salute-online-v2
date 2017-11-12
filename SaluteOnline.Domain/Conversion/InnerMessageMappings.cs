using SaluteOnline.Domain.Domain.EF;
using SaluteOnline.Domain.DTO.InnerMessage;

namespace SaluteOnline.Domain.Conversion
{
    public static class InnerMessageMappings
    {
        public static InnerMessageDto ToDto(this InnerMessage message)
        {
            return new InnerMessageDto
            {
                Id = message.Id,
                Status = message.Status,
                LastActivity = message.LastActivity,
                Title = message.Title,
                ReceiverType = message.ReceiverType,
                ReceiverId = message.ReceiverId,
                SenderType = message.SenderType,
                SentBySystem = message.SentBySystem,
                Created = message.Created,
                Body = message.Body,
                OneResponseForAll = message.OneResponseForAll,
                SenderId = message.SenderId
            };
        }
    }
}

using System.Collections.Generic;
using SaluteOnline.Domain.Domain;
using SaluteOnline.Domain.DTO.InnerMessage;

namespace SaluteOnline.API.Services.Interface
{
    public interface IMessageService
    {
        Page<InnerMessageDto> GetMessages(InnerMessagesFilter filter, string email);
    }
}
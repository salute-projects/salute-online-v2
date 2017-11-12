using System.Collections.Generic;
using SaluteOnline.Domain.DTO.InnerMessage;

namespace SaluteOnline.API.Services.Interface
{
    public interface IMessageService
    {
        IEnumerable<InnerMessageDto> GetMessages(InnerMessagesFilter filter, string email);
    }
}
using System;
using System.Collections.Generic;
using SaluteOnline.Domain.Domain;
using SaluteOnline.Domain.DTO;
using SaluteOnline.Domain.DTO.Chat;

namespace SaluteOnline.API.Services.Interface
{
    public interface IChatService
    {
        List<ChatDto> GetMyChats(string email);
        void PostPrivateMessage(PostPrivateMessageDto dto);
        List<ChatMessageDto> GetLatestMessages(int take, string email);
        Page<ChatMessageDto> LoadChatMessages(BaseFilter filter, Guid chatGuid, string email);
    }
}
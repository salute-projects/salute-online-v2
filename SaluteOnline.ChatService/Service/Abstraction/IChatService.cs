using System;
using System.Collections.Generic;
using SaluteOnline.ChatService.Domain.DTO;
using SaluteOnline.Domain.Domain;
using SaluteOnline.Domain.DTO;

namespace SaluteOnline.ChatService.Service.Abstraction
{
    public interface IChatService
    {
        List<ChatDto> GetMyChats(string subjectId);
        void PostPrivateMessage(PostPrivateMessageDto dto);
        List<ChatMessageDto> GetLatestMessages(int take, string email);
        Page<ChatMessageDto> LoadChatMessages(BaseFilter filter, Guid chatGuid, string email);
    }
}
using System;
using System.Collections.Generic;
using SaluteOnline.ChatService.Service.Abstraction;
using SaluteOnline.Domain.Domain;
using SaluteOnline.Domain.DTO;
using SaluteOnline.Domain.DTO.Chat;

namespace SaluteOnline.ChatService.Service.Implementation
{
    public class ChatService : IChatService
    {
        public List<ChatDto> GetMyChats(string email)
        {
            throw new NotImplementedException();
        }

        public void PostPrivateMessage(PostPrivateMessageDto dto)
        {
            throw new NotImplementedException();
        }

        public List<ChatMessageDto> GetLatestMessages(int take, string email)
        {
            throw new NotImplementedException();
        }

        public Page<ChatMessageDto> LoadChatMessages(BaseFilter filter, Guid chatGuid, string email)
        {
            throw new NotImplementedException();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Mapster;
using Microsoft.Extensions.Logging;
using SaluteOnline.ChatService.DAL;
using SaluteOnline.ChatService.Domain;
using SaluteOnline.ChatService.Domain.DTO;
using SaluteOnline.ChatService.Service.Abstraction;
using SaluteOnline.Shared.Common;
using SaluteOnline.Shared.Exceptions;

namespace SaluteOnline.ChatService.Service.Implementation
{
    public class ChatService : IChatService
    {
        private readonly ILogger<ChatService> _logger;
        private readonly IGenericRepository<Chat> _chatRepository;
        private readonly IGenericRepository<ChatMember> _chatMemberRepository;

        public ChatService(IGenericRepository<Chat> chatRepository, IGenericRepository<ChatMember> chatMemberRepository, ILogger<ChatService> logger)
        {
            _logger = logger;
            _chatRepository = chatRepository;
            _chatMemberRepository = chatMemberRepository;
        }

        public List<ChatDto> GetMyChats(string subjectId)
        {
            try
            {
                if (!Guid.TryParse(subjectId, out Guid myGuid))
                    throw new SoException("Wrong identifier", HttpStatusCode.BadRequest);

                var result = new List<ChatDto>();
                var me = _chatMemberRepository.GetAsQueryable(t => t.Guid == myGuid)?.FirstOrDefault();
                if (me == null)
                    return result;

                var myChats = _chatRepository.GetAsQueryable(t => t.Participants != null && t.Participants.Any(x => x == myGuid))
                        .OrderByDescending(t => t.LastUpdated);
                foreach (var chat in myChats)
                {
                    var newItem = chat.Adapt<ChatDto>();
                    var participants =
                        chat.Participants.Select(
                            x => _chatMemberRepository.GetAsQueryable(t => t.Guid == x).FirstOrDefault()).ToList();
                    if (!participants.Any())
                        continue;
                    if (!chat.IsPrivate)
                    {
                        newItem.Title = chat.Title;
                        newItem.Avatar = chat.Avatar;
                    }
                    else
                    {
                        var other = participants.FirstOrDefault(t => t.Guid != myGuid);
                        newItem.Title = other?.Type == EntityType.System ? "SALUTE ONLINE" : other?.Title;
                    }
                    newItem.Participants = participants.Select(t => new ChatMemberDto
                    {
                        Type = t.Type,
                        SubjectId = t.Guid,
                        Title = t.Type == EntityType.System ? "SALUTE ONLINE" : t.Title
                    }).ToList();

                    newItem.NewMessages =
                        chat.Messages.Count(t => t.Seen.All(x => x.Observer != me.Guid) && t.Sender != me.Guid);

                    result.Add(newItem);
                }
                return result;
            }
            catch (Exception e)
            {
                if (e is SoException)
                    throw;
                _logger.LogError(e.Message);
                throw new SoException("Error while loading list of chats", HttpStatusCode.InternalServerError);
            }
        }

        public void PostPrivateMessage(PostPrivateMessageDto dto)
        {
            try
            {
                if (string.IsNullOrEmpty(dto?.Message))
                    throw new SoException("Empty message not allowed", HttpStatusCode.BadRequest);

                if (string.IsNullOrEmpty(dto.SenderGuid) || string.IsNullOrEmpty(dto.ReceiverGuid) 
                    || !Guid.TryParse(dto.SenderGuid, out Guid senderGuid) || !Guid.TryParse(dto.ReceiverGuid, out Guid receivedGuid))
                    throw new SoException("Wrong input model", HttpStatusCode.BadRequest);

                var existingSender = _chatMemberRepository.GetAsQueryable(t => t.Guid == senderGuid).FirstOrDefault();
                var existingReceiver = _chatMemberRepository.GetAsQueryable(t => t.Guid == receivedGuid).FirstOrDefault();

                if (existingSender == null)
                {
                    var senderMember = new ChatMember
                    {
                        Type = dto.SenderType,
                        Guid = senderGuid,
                        Registered = DateTimeOffset.UtcNow,
                        Title = dto.SenderTitle
                    };
                    _chatMemberRepository.Insert(senderMember);
                }
                if (existingReceiver == null)
                {
                    var receiverMember = new ChatMember
                    {
                        Guid = receivedGuid,
                        Type = dto.ReceiverType,
                        Title = dto.ReceiverTitle,
                        Registered = DateTimeOffset.UtcNow
                    };
                    _chatMemberRepository.Insert(receiverMember);
                }
                var newMessage = new ChatMessage
                {
                    Message = dto.Message,
                    Sent = DateTimeOffset.UtcNow,
                    Sender = senderGuid,
                    Seen = new List<ChatSeenStamp>()
                };
                if (dto.ChatGuid == null || dto.ChatGuid == default(Guid))
                {
                    var newChat = new Chat
                    {
                        Guid = Guid.NewGuid(),
                        Title = string.Empty,
                        Avatar = string.Empty,
                        Participants = new List<Guid> {senderGuid, receivedGuid},
                        Created = DateTimeOffset.UtcNow,
                        Messages = new List<ChatMessage> {newMessage},
                        IsPrivate = true,
                        LastUpdated = DateTimeOffset.UtcNow
                    };
                    _chatRepository.Insert(newChat);
                }
                else
                {
                    var existingChat = _chatRepository.GetAsQueryable(t => t.Guid == dto.ChatGuid)?.FirstOrDefault();
                    if (existingChat == null)
                        throw new SoException("Wrong chat identifier", HttpStatusCode.BadRequest);
                    existingChat.Messages.Add(newMessage);
                    _chatRepository.Update(existingChat);
                }
            }
            catch (Exception e)
            {
                if (e is SoException)
                    throw;
                _logger.LogError(e.Message);
                throw new SoException("Error while loading list of chats", HttpStatusCode.InternalServerError);
            }
        }

        public List<ChatMessageDto> GetLatestMessages(int take, string subjectId)
        {
            try
            {
                if (take <= 0)
                    throw new SoException("Wrong request. Fetch number must be greater than zero",
                        HttpStatusCode.BadRequest);

                if (!Guid.TryParse(subjectId, out Guid myGuid))
                    throw new SoException("Wrong input model", HttpStatusCode.BadRequest);

                var allChats = _chatRepository.GetAsQueryable(t => t.Participants.Any(x => x == myGuid)).ToList();
                if (!allChats.Any())
                    return new List<ChatMessageDto>();

                var result = allChats.SelectMany(t => t.Messages, (guid, message) => new {guid.Guid, message})
                    .Where(t => t.message.Sender != myGuid)
                    .OrderByDescending(t => t.message.Sent)
                    .Take(take)
                    .Select(t => new ChatMessageDto
                    {
                        Guid = t.Guid,
                        Sent = t.message.Sent,
                        Message = t.message.Message,
                        Sender = t.message.Sender,
                        Seen = t.message.Seen.Any(x => x.Observer == myGuid),
                        My = t.message.Sender == myGuid
                    }).ToList();
                foreach (var message in result)
                {
                    message.SenderType = _chatMemberRepository.GetById(message.Sender)?.Type ?? EntityType.System;
                }
                return result;
            }
            catch (Exception e)
            {
                if (e is SoException)
                    throw;
                _logger.LogError(e.Message);
                throw new SoException("Error while loading messages", HttpStatusCode.InternalServerError);
            }
        }

        public Page<ChatMessageDto> LoadChatMessages(BaseFilter filter, Guid chatGuid, string subjectId)
        {
            try
            {
                if (chatGuid == default(Guid))
                    throw new SoException("Wrong chat identifier", HttpStatusCode.BadRequest);

                if (!Guid.TryParse(subjectId, out var myGuid))
                    throw new SoException("Wrong user identifier", HttpStatusCode.BadRequest);

                if (filter == null)
                    throw new SoException("Arguments omitted", HttpStatusCode.BadRequest);

                var chat = _chatRepository.GetById(chatGuid);
                if (chat == null)
                    throw new SoException("Chat not found", HttpStatusCode.BadRequest);

                var result =
                    chat.Messages.Skip(filter.Page * filter.PageSize ?? 5)
                        .Take(filter.PageSize ?? 5)
                        .Select(t => new ChatMessageDto
                        {
                            Sent = t.Sent,
                            Message = t.Message,
                            Guid = chatGuid,
                            My = t.Sender == myGuid,
                            Seen = t.Seen.Any(x => x.Observer == myGuid),
                            Sender = t.Sender,
                        }).ToList();
                foreach (var message in result)
                {
                    message.SenderType = _chatMemberRepository.GetById(message.Sender)?.Type ?? EntityType.System;
                }
                return new Page<ChatMessageDto>(filter.Page, filter.PageSize ?? 5, chat.Messages.Count, result); 
            }
            catch (Exception e)
            {
                if (e is SoException)
                    throw;
                _logger.LogError(e.Message);
                throw new SoException("Error while loading messages", HttpStatusCode.InternalServerError);
            }
        }
    }
}

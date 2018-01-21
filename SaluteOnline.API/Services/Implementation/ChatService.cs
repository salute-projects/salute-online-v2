using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using SaluteOnline.API.DAL;
using SaluteOnline.API.Hub;
using SaluteOnline.API.Services.Interface;
using SaluteOnline.Domain.Domain;
using SaluteOnline.Domain.Domain.Mongo.Chat;
using SaluteOnline.Domain.DTO;
using SaluteOnline.Domain.DTO.Chat;
using SaluteOnline.Domain.Exceptions;

namespace SaluteOnline.API.Services.Implementation
{
    public class ChatService : IChatService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ChatService> _logger;
        private readonly IHubContext<SoMessageHub> _messageHub;

        public ChatService(IUnitOfWork unitOfWork, ILogger<ChatService> logger, IHubContext<SoMessageHub> messageHub)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _messageHub = messageHub;
        }

        public List<ChatDto> GetMyChats(string email)
        {
            try
            {
                var currentUser = _unitOfWork.Users.Get(t => t.Email == email).FirstOrDefault();
                if (currentUser == null)
                    throw new SoException("Not authorized", HttpStatusCode.Unauthorized);
                var chats =
                    _unitOfWork.Chats.GetAsQueryable(t => t.Participants.Any(x => x.Id == currentUser.Id))
                        .OrderBy(t => t.LastUpdated);
                var results = new List<ChatDto>();
                foreach (var chat in chats)
                {
                    var newItem = new ChatDto
                    {
                        Guid = chat.Guid,
                        Created = chat.Created,
                        LastUpdated = chat.LastUpdated,
                        IsPrivate = chat.IsPrivate
                    };
                    var allUserParticipants =
                        chat.Participants.Where(t => t.Type == EntityType.User)
                            .Select(t => _unitOfWork.Users.GetById(id: t.Id))
                            .ToList();
                    var allClubsParticipants =
                        chat.Participants.Where(t => t.Type == EntityType.Club)
                            .Select(t => _unitOfWork.Clubs.GetById(id: t.Id))
                            .ToList();
                    if (!chat.IsPrivate)
                    {
                        newItem.Title = chat.Title;
                        newItem.Avatar = chat.Avatar;
                    }
                    else
                    {
                        var other = chat.Participants.FirstOrDefault(t => t.Id != currentUser.Id);
                        if (other == null)
                            continue;
                        newItem.Title = other.Type == EntityType.User
                            ? allUserParticipants.FirstOrDefault(t => t.Id == other.Id)?.DisplayName
                            : other.Type == EntityType.Club
                                ? allClubsParticipants.FirstOrDefault(t => t.Id == other.Id)?.Title
                                : "SALUTE ONLINE";
                        newItem.Avatar = other.Type == EntityType.User
                            ? allUserParticipants.FirstOrDefault(t => t.Id == other.Id)?.Avatar
                            : other.Type == EntityType.Club
                                ? allClubsParticipants.FirstOrDefault(t => t.Id == other.Id)?.Logo
                                : string.Empty;
                    }
                    newItem.Participants = chat.Participants.Select(t => new ChatMemberDto
                    {
                        Id = t.Id,
                        Type = t.Type,
                        Title = t.Type == EntityType.User
                            ? allUserParticipants.FirstOrDefault(x => x.Id == t.Id)?.DisplayName
                            : t.Type == EntityType.Club
                                ? allClubsParticipants.FirstOrDefault(x => x.Id == t.Id)?.Title
                                : "SALUTE ONLINE"
                    }).ToList();
                    newItem.NewMessages =
                        chat.Messages.Count(
                            t =>
                                t.Seen.All(x => x.ObserverId != currentUser.Id) &&
                                t.Sender.Id != currentUser.Id);
                    results.Add(newItem);
                }
                return results;
            }
            catch (SoException)
            {
                throw;
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                throw new SoException("Error while loading chats. Please try a bit later", HttpStatusCode.InternalServerError);
            }
        }

        public void PostPrivateMessage(PostPrivateMessageDto dto)
        {
            try
            {
                if (dto == null)
                    throw new SoException("Arguments omitted", HttpStatusCode.BadRequest);
                var newChatMessage = new ChatMessage
                {
                    Message = dto.Message,
                    Sent = DateTimeOffset.UtcNow,
                    Sender = new ChatMember
                    {
                        Id = dto.SenderId,
                        Type = dto.SenderType
                    }
                };
                var existingPrivateChat =
                    _unitOfWork.Chats.Get(
                        t => t.Participants.Any(x => x.Id == dto.SenderId && x.Type == dto.SenderType) &&
                            t.Participants.Any(x => x.Id == dto.ReceiverId && x.Type == dto.ReceiverType) && t.IsPrivate).SingleOrDefault();
                if (existingPrivateChat == null)
                {
                    var newChat = new Chat
                    {
                        Guid = Guid.NewGuid(),
                        Created = DateTimeOffset.UtcNow,
                        IsPrivate = true,
                        LastUpdated = DateTimeOffset.UtcNow,
                        Id = _unitOfWork.Chats.Count() + 1,
                        Participants = new List<ChatMember>
                        {
                            new ChatMember { Id = dto.SenderId, Type = dto.SenderType },
                            new ChatMember { Id = dto.ReceiverId, Type = dto.ReceiverType }
                        },
                        Messages = new List<ChatMessage>
                        {
                            newChatMessage
                        }
                    };
                    _unitOfWork.Chats.Insert(newChat);
                    return;
                }
                existingPrivateChat.LastUpdated = DateTimeOffset.UtcNow;
                existingPrivateChat.Messages.Add(newChatMessage);
                _unitOfWork.Chats.Update(existingPrivateChat);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                throw new SoException("Error while loading messages. Please try a bit later", HttpStatusCode.InternalServerError);
            }
        }

        public List<ChatMessageDto> GetLatestMessages(int take, string email)
        {
            try
            {
                var currentUser = _unitOfWork.Users.Get(t => t.Email == email).FirstOrDefault();
                if (currentUser == null)
                    throw new SoException("Not authorized", HttpStatusCode.Unauthorized);
                if (take <= 0)
                    throw new SoException("Wrong request. Fetch number must be greater than zero", HttpStatusCode.BadRequest);
                var allChats =
                    _unitOfWork.Chats.GetAsQueryable(
                        t => t.Participants.Any(x => x.Id == currentUser.Id && x.Type == EntityType.User));
                if (allChats == null)
                    return new List<ChatMessageDto>();
                var result = allChats.SelectMany(t => t.Messages, (guid, message) => new {guid.Guid, message})
                    .Where(t => t.message.Sender.Id != currentUser.Id)
                    .OrderByDescending(t => t.message.Sent)
                    .Take(take)
                    .Select(t => new ChatMessageDto
                    {
                        Guid = t.Guid,
                        Sent = t.message.Sent,
                        Message = t.message.Message,
                        SenderType = t.message.Sender.Type,
                        SenderId = t.message.Sender.Id,
                        Seen = t.message.Seen.Any(x => x.ObserverId == currentUser.Id),
                        My = t.message.Sender.Id == currentUser.Id && t.message.Sender.Type == EntityType.User
                    }).ToList();
                foreach (var message in result)
                {
                    message.Avatar = message.SenderType == EntityType.User
                        ? _unitOfWork.Users.GetById(id: message.SenderId)?.Avatar
                        : message.SenderType == EntityType.Club
                            ? _unitOfWork.Clubs.GetById(id: message.SenderId)?.Logo
                            : string.Empty;
                }
                return result;
            }
            catch (SoException)
            {
                throw;
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                throw new SoException("Error while loading messages. Please try a bit later", HttpStatusCode.InternalServerError);
            }
        }

        public Page<ChatMessageDto> LoadChatMessages(BaseFilter filter, Guid chatGuid, string email)
        {
            try
            {
                var currentUser = _unitOfWork.Users.Get(t => t.Email == email).FirstOrDefault();
                if (currentUser == null)
                    throw new SoException("Not authorized", HttpStatusCode.Unauthorized);
                if (filter == null)
                    throw new SoException("Wrong request. Filter must be defined", HttpStatusCode.BadRequest);
                if (chatGuid == default(Guid))
                    throw new SoException("Wrong chat identifier", HttpStatusCode.BadRequest);
                var chat = _unitOfWork.Chats.GetById(chatGuid);
                if (chat == null)
                    throw new SoException("Chat not found", HttpStatusCode.NotFound);
                var result =
                    chat.Messages.Skip(filter.Page * filter.PageSize ?? 5)
                        .Take(filter.PageSize ?? 5)
                        .Select(t => new ChatMessageDto
                        {
                            Sent = t.Sent,
                            Message = t.Message,
                            Guid = chatGuid,
                            My = t.Sender.Type == EntityType.User && t.Sender.Id == currentUser.Id,
                            Seen = t.Seen.Any(x => x.ObserverId == currentUser.Id),
                            SenderId = t.Sender.Id,
                            SenderType = t.Sender.Type,
                            Avatar =
                                t.Sender.Type == EntityType.User
                                    ? _unitOfWork.Users.GetById(id: t.Sender.Id).Avatar
                                    : t.Sender.Type == EntityType.Club
                                        ? _unitOfWork.Clubs.GetById(id: t.Sender.Id).Logo
                                        : string.Empty
                        });
                return new Page<ChatMessageDto>(filter.Page, filter.PageSize ?? 5, chat.Messages.Count, result);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                throw new SoException("Error while loading messages. Please try a bit later", HttpStatusCode.InternalServerError);
            }
        }

        public void SendToAllViaHub(string message)
        {
            _messageHub.Clients.All.InvokeAsync("newMessage", new { message, sent = DateTimeOffset.UtcNow });
        }
    }
}

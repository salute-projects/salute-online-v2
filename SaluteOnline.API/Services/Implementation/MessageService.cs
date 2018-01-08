using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SaluteOnline.API.DAL;
using SaluteOnline.API.Hub;
using SaluteOnline.API.Services.Interface;
using SaluteOnline.Domain.Conversion;
using SaluteOnline.Domain.Domain;
using SaluteOnline.Domain.DTO;
using SaluteOnline.Domain.DTO.InnerMessage;

namespace SaluteOnline.API.Services.Implementation
{
    public class MessageService : IMessageService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger _logger;
        private readonly IHubContext<SoMessageHub> _messageHub;

        public MessageService(IUnitOfWork unitOfWork, ILogger<ClubsService> logger, IHubContext<SoMessageHub> messageHub)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _messageHub = messageHub;
        }

        public Page<InnerMessageDto> GetMessages(InnerMessagesFilter filter, string email)
        {
            try
            {
                var currentUser = _unitOfWork.Users.Get(t => t.Email == email).FirstOrDefault();
                if (currentUser == null)
                    throw new ArgumentException("Internal error happened. Please try a bit later");
                List<InnerMessageDto> result;
                switch (filter.ReceiverType)
                {
                    case EntityType.User:
                        var targetUser = _unitOfWork.Users.GetAsQueryable(t => t.Id == currentUser.Id)
                            .Include(t => t.InnerMessagesReceived)
                            .Include(t => t.ClubsAdministrated)
                            .ThenInclude(t => t.Club)
                            .ThenInclude(t => t.InnerMessagesReceived)
                            .SingleOrDefault();
                        if (targetUser == null || targetUser.Id != currentUser.Id)
                            throw new ArgumentException("Operation not allowed");

                        result = targetUser.InnerMessagesReceived.Where(t => t.Status == filter.Status).Select(t => t.ToDto()).ToList();
                        if (currentUser.Role != Role.User)
                        {
                            result = targetUser.ClubsAdministrated.Select(t => t.Club)
                                .SelectMany(t => t.InnerMessagesReceived)
                                .Where(t => t.Status == filter.Status)
                                .Select(t => t.ToDto())
                                .Concat(result)
                                .ToList();
                        }
                        break;
                    case EntityType.Club:
                        var targetClub =
                            _unitOfWork.Clubs.GetAsQueryable(t => t.Id == filter.ReceiverId)
                                .Include(t => t.Administrators)
                                .ThenInclude(t => t.User).SingleOrDefault();
                        if (targetClub == null || targetClub.Administrators.All(t => t.UserId != currentUser.Id))
                            throw new ArgumentException("Operation not allowed");
                        result = targetClub.InnerMessagesReceived
                            .Where(t => t.Status == filter.Status)
                            .Select(t => t.ToDto())
                            .ToList();
                        break;
                    case EntityType.System:
                        throw new ArgumentException("Operation not allowed");
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                foreach (var message in result)
                {
                    switch (message.SenderType)
                    {
                        case EntityType.User:
                            var user = _unitOfWork.Users.GetById(id: message.SenderId);
                            message.Avatar = user?.Avatar;
                            message.SenderName = string.IsNullOrEmpty(user?.Nickname) ? $"{user?.FirstName} {user?.LastName}" : user.Nickname;
                            break;
                        case EntityType.Club:
                            var club = _unitOfWork.Clubs.GetById(id: message.SenderId);
                            message.Avatar = club?.Logo;
                            message.SenderName = club?.Title;
                            break;
                        case EntityType.System:
                            message.Avatar = string.Empty;
                            message.SenderName = "Salute Online";
                            break;
                        default:
                            message.Avatar = string.Empty;
                            break;
                    }
                }
                var slice =
                    result.OrderByDescending(t => t.Created)
                        .Skip((filter.Page - 1) * (filter.PageSize ?? 25))
                        .Take(filter.PageSize ?? 25);

                return new Page<InnerMessageDto>(filter.Page, filter.PageSize ?? 25, result.Count, slice);
            }
            catch (ArgumentException)
            {
                throw;
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                throw new Exception("Error while loading messages. Please try a bit later");
            }
        }

        public IEnumerable<InnerMessageSenderSummary> GetSendersForUser(InnerMessageSenderFilter filter, string email)
        {
            try
            {
                var currentUser = _unitOfWork.Users.Get(t => t.Email == email).FirstOrDefault();
                if (currentUser == null)
                    throw new ArgumentException("Internal error happened. Please try a bit later");
                var targetUser = _unitOfWork.Users.GetAsQueryable(t => t.Id == currentUser.Id)
                            .Include(t => t.InnerMessagesReceived)
                            .Include(t => t.ClubsAdministrated)
                            .ThenInclude(t => t.Club)
                            .ThenInclude(t => t.InnerMessagesReceived)
                            .SingleOrDefault();
                if (targetUser == null || targetUser.Id != currentUser.Id)
                    throw new ArgumentException("Operation not allowed");
                var result = targetUser.InnerMessagesReceived.Where(t => t.Status == filter.Status).Select(t => t.ToSenderSummaryDto()).ToList();
                if (currentUser.Role != Role.User)
                {
                    result = targetUser.ClubsAdministrated.Select(t => t.Club)
                        .SelectMany(t => t.InnerMessagesReceived)
                        .Where(t => t.Status == filter.Status)
                        .Select(t => t.ToSenderSummaryDto())
                        .Concat(result)
                        .ToList();
                }
                foreach (var message in result)
                {
                    switch (message.SenderType)
                    {
                        case EntityType.User:
                            var user = _unitOfWork.Users.GetById(id: message.SenderId);
                            message.Avatar = user?.Avatar;
                            message.Title = string.IsNullOrEmpty(user?.Nickname) ? $"{user?.FirstName} {user?.LastName}" : user.Nickname;
                            break;
                        case EntityType.Club:
                            var club = _unitOfWork.Clubs.GetById(id: message.SenderId);
                            message.Avatar = club?.Logo;
                            message.Title = club?.Title;
                            break;
                        case EntityType.System:
                            message.Avatar = string.Empty;
                            message.Title = "Salute Online";
                            break;
                        default:
                            message.Avatar = string.Empty;
                            break;
                    }
                }
                return result;
            }
            catch (ArgumentException)
            {
                throw;
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                throw new Exception("Error while loading messages. Please try a bit later");
            }
        }

        public void SendToAllViaHub(string message)
        {
            _messageHub.Clients.All.InvokeAsync("newMessage", new { message, sent = DateTimeOffset.UtcNow });
        }
    }
}

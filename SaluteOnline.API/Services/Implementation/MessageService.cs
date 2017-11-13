using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SaluteOnline.API.DAL;
using SaluteOnline.API.Services.Interface;
using SaluteOnline.Domain.Conversion;
using SaluteOnline.Domain.Domain.EF;
using SaluteOnline.Domain.DTO;
using SaluteOnline.Domain.DTO.InnerMessage;

namespace SaluteOnline.API.Services.Implementation
{
    public class MessageService : IMessageService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger _logger;

        public MessageService(IUnitOfWork unitOfWork, ILogger<ClubsService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public IEnumerable<InnerMessageDto> GetMessages(InnerMessagesFilter filter, string email)
        {
            try
            {
                var currentUser = _unitOfWork.Users.Get(t => t.Email == email).FirstOrDefault();
                if (currentUser == null)
                    throw new ArgumentException("Internal error happened. Please try a bit later");
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
                        var forMe = targetUser.InnerMessagesReceived.Select(t => t.ToDto());
                        if (currentUser.Role == Role.User)
                            return forMe;
                        return targetUser.ClubsAdministrated.Select(t => t.Club)
                                .SelectMany(t => t.InnerMessagesReceived)
                                .Select(t => t.ToDto()).Concat(forMe);
                    case EntityType.Club:
                        var targetClub =
                            _unitOfWork.Clubs.GetAsQueryable(t => t.Id == filter.ReceiverId)
                                .Include(t => t.Administrators)
                                .ThenInclude(t => t.User).SingleOrDefault();
                        if (targetClub == null || targetClub.Administrators.All(t => t.UserId != currentUser.Id))
                            throw new ArgumentException("Operation not allowed");
                        return targetClub.InnerMessagesReceived.Select(t => t.ToDto());
                    case EntityType.System:
                        throw new ArgumentException("Operation not allowed");
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            catch (ArgumentException)
            {
                throw;
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                throw new Exception("Error while adding new club. Please try a bit later");
            }
        }
    }
}

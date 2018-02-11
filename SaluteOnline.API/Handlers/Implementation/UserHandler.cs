using System;
using System.Linq;
using Microsoft.Extensions.Logging;
using RawRabbit;
using SaluteOnline.API.DAL;
using SaluteOnline.API.Handlers.Declaration;
using SaluteOnline.Domain.Domain.EF;
using SaluteOnline.Domain.DTO;
using SaluteOnline.Domain.DTO.Activity;

namespace SaluteOnline.API.Handlers.Implementation
{
    public class UserHandler : IUserHandler
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UserHandler> _logger;
        private readonly IBusClient _busClient;

        public UserHandler(IUnitOfWork unitOfWork, ILogger<UserHandler> logger, IBusClient busClient)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _busClient = busClient;
        }

        public bool HandleNewUser(UserRegisteredEvent data)
        {
            try
            {
                if (_unitOfWork.Users.GetAsQueryable(t => t.SubjectId == data.SubjectId).FirstOrDefault() != null)
                    return false;
                var newUser = new User
                {
                    SubjectId = data.SubjectId,
                    Email = data.Email,
                    Nickname = data.Username,
                    Guid = Guid.NewGuid(),
                    LastActivity = DateTimeOffset.UtcNow,
                    IsActive = true,
                    Registered = DateTimeOffset.UtcNow,
                    Role = Role.User
                };
                _unitOfWork.Users.Insert(newUser);
                _unitOfWork.Save();
                _busClient.PublishAsync(new UserCreatedEvent
                {
                    SubjectId = data.SubjectId,
                    UserId = newUser.Id
                });
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return false;
            }
        }
    }
}

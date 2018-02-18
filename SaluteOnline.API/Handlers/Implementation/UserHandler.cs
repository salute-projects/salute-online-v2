using System;
using Microsoft.Extensions.Logging;
using RawRabbit;
using SaluteOnline.API.DAL;
using SaluteOnline.API.Domain;
using SaluteOnline.API.Handlers.Declaration;
using SaluteOnline.Shared.Common;
using SaluteOnline.Shared.Events;

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
                if (!Guid.TryParse(data.SubjectId, out var userGuid))
                    return false;
                if (_unitOfWork.Users.GetById(userGuid) != null)
                    return false;

                var newUser = new User
                {
                    Email = data.Email,
                    Nickname = data.Username,
                    Guid = userGuid,
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

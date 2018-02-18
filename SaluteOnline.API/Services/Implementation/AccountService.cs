using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using SaluteOnline.API.DAL;
using SaluteOnline.API.Domain.Mapping;
using SaluteOnline.API.DTO.User;
using SaluteOnline.API.Services.Interface;
using SaluteOnline.Shared.Exceptions;

namespace SaluteOnline.API.Services.Implementation
{
    public class AccountService : IAccountService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<AccountService> _logger;

        public AccountService(IUnitOfWork unitOfWork, ILogger<AccountService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public UserDto GetUserInfo(string subjectId)
        {
            try
            {
                if (!Guid.TryParse(subjectId, out var userGuid)) 
                    throw new SoException("Corrupted token", HttpStatusCode.Unauthorized);

                var user = _unitOfWork.Users.GetById(userGuid);
                if (user == null)
                    throw new SoException("User not found", HttpStatusCode.BadRequest);

                return user.Adapt<UserDto>();
            }
            catch (Exception ex)
            {
                if (ex is SoException)
                    throw;
                _logger.LogError(ex.Message);
                throw new SoException("Error while getting user info. Please try a bit later", HttpStatusCode.InternalServerError);
            }
        }

        public void UpdateUserInfo(UserDto user, string subjectId)
        {
            try
            {
                if (!Guid.TryParse(subjectId, out var userGuid)) 
                    throw new SoException("Corrupted token", HttpStatusCode.Unauthorized);

                var existing = _unitOfWork.Users.GetById(userGuid);
                if (existing == null)
                    throw new SoException("User does not exists", HttpStatusCode.BadRequest);

                user.UpdateEntity(existing);
                _unitOfWork.Users.Update(existing);
                _unitOfWork.Save();
            }
            catch (Exception e)
            {
                if (e is SoException)
                    throw;
                _logger.LogError(e.Message);
                throw new SoException("Error while updating user info. Please try a bit later", HttpStatusCode.InternalServerError);
            }
        }

        public void UpdateMainUserInfo(UserMainInfoDto user, string subjectId)
        {
            try
            {
                if (!Guid.TryParse(subjectId, out var userGuid))
                    throw new SoException("Corrupted token", HttpStatusCode.Unauthorized);

                var existing = _unitOfWork.Users.GetById(userGuid);
                if (existing == null)
                    throw new SoException("User does not exists", HttpStatusCode.BadRequest);

                user.UpdateEntity(existing);
                _unitOfWork.Users.Update(existing);
                _unitOfWork.Save();
            }
            catch (Exception e)
            {
                if (e is SoException)
                    throw;
                _logger.LogError(e.Message);
                throw new SoException("Error while updating user info. Please try a bit later", HttpStatusCode.InternalServerError);
            }
        }

        public void UpdatePersonalUserInfo(UserPersonalInfoDto user, string subjectId)
        {
            try
            {
                if (!Guid.TryParse(subjectId, out var userGuid))
                    throw new SoException("Corrupted token", HttpStatusCode.Unauthorized);

                var existing = _unitOfWork.Users.GetById(userGuid);
                if (existing == null)
                    throw new SoException("User does not exists", HttpStatusCode.BadRequest);

                user.UpdateEntity(existing);
                _unitOfWork.Users.Update(existing);
                _unitOfWork.Save();
            }
            catch (Exception e)
            {
                if (e is SoException)
                    throw;
                _logger.LogError(e.Message);
                throw new SoException("Error while updating user info. Please try a bit later", HttpStatusCode.InternalServerError);
            }
        }

        public async Task<string> UpdateUserAvatar(IFormFile avatar, string subjectId)
        {
            try
            {
                if (avatar == null || avatar.Length == 0 || string.IsNullOrEmpty(subjectId))
                    throw new SoException("File wasn't uploaded", HttpStatusCode.BadRequest);

                if (!Guid.TryParse(subjectId, out var userGuid))
                    throw new SoException("Corrupted token", HttpStatusCode.Unauthorized);

                var existing = _unitOfWork.Users.GetById(userGuid);
                if (existing == null)
                    throw new SoException("Could not found user", HttpStatusCode.BadRequest);

                using (var stream = new MemoryStream())
                {
                    await avatar.CopyToAsync(stream);
                    existing.Avatar = Convert.ToBase64String(stream.ToArray());
                    existing.LastActivity = DateTimeOffset.UtcNow;
                    _unitOfWork.Save();
                    return existing.Avatar;
                }
            }
            catch (Exception e)
            {
                if (e is SoException)
                    throw;
                _logger.LogError(e.Message);
                throw new SoException("Error while getting user info. Please try a bit later", HttpStatusCode.InternalServerError);
            }
        }
    }
}

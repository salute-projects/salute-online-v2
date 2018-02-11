using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using SaluteOnline.API.DAL;
using SaluteOnline.API.Services.Interface;
using SaluteOnline.Domain.Conversion;
using SaluteOnline.Domain.DTO.User;

namespace SaluteOnline.API.Services.Implementation
{
    public class AccountService : IAccountService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<AccountService> _logger;
        private readonly IBusService _busService;

        public AccountService(IUnitOfWork unitOfWork, ILogger<AccountService> logger, IBusService busService)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _busService = busService;
        }

        public bool UserExists(string subjectId)
        {
            try
            {
                return _unitOfWork.Users.Count(t => string.Equals(t.SubjectId, subjectId, StringComparison.OrdinalIgnoreCase)) != 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw new Exception("Error while getting user. Please try a bit later");
            }
        }

        public UserDto GetUserInfo(string subjectId)
        {
            try
            {
                var user = _unitOfWork.Users.Get(t => t.SubjectId == subjectId).FirstOrDefault();
                if (user == null)
                    throw new ArgumentException("User not found");
                return user.Adapt<UserDto>();
            }
            catch (ArgumentException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw new Exception("Error while getting user info. Please try a bit later");
            }
        }

        public void UpdateUserInfo(UserDto user, string subjectId)
        {
            try
            {
                var existing = _unitOfWork.Users.GetById(id: user.Id);
                if (existing == null)
                    throw new ArgumentException("User does not exists");
                if (existing.SubjectId != subjectId)
                    throw new ArgumentException("Operation not allowed");
                user.UpdateEntity(existing);
                _unitOfWork.Users.Update(existing);
                _unitOfWork.Save();
            }
            catch (ArgumentException)
            {
                throw;
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                throw new Exception("Error while updating user info. Please try a bit later");
            }
        }

        public void UpdateMainUserInfo(UserMainInfoDto user, string subjectId)
        {
            try
            {
                var existing = _unitOfWork.Users.GetById(id: user.Id);
                if (existing == null)
                    throw new ArgumentException("User does not exists");
                if (existing.SubjectId != subjectId)
                    throw new ArgumentException("Operation not allowed");
                user.UpdateEntity(existing);
                _unitOfWork.Users.Update(existing);
                _unitOfWork.Save();
            }
            catch (ArgumentException)
            {
                throw;
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                throw new Exception("Error while updating user info. Please try a bit later");
            }
        }

        public void UpdatePersonalUserInfo(UserPersonalInfoDto user, string subjectId)
        {
            try
            {
                var existing = _unitOfWork.Users.GetById(id: user.Id);
                if (existing == null)
                    throw new ArgumentException("User does not exists");
                if (existing.SubjectId != subjectId)
                    throw new ArgumentException("Operation not allowed");
                user.UpdateEntity(existing);
                _unitOfWork.Users.Update(existing);
                _unitOfWork.Save();
            }
            catch (ArgumentException)
            {
                throw;
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                throw new Exception("Error while updating user info. Please try a bit later");
            }
        }

        public async Task<string> UpdateUserAvatar(IFormFile avatar, string subjectId)
        {
            try
            {
                if (avatar == null || avatar.Length == 0 || string.IsNullOrEmpty(subjectId))
                    throw new ArgumentException("File wasn't uploaded");
                var existing = _unitOfWork.Users.Get(t => t.SubjectId == subjectId).SingleOrDefault();
                if (existing == null)
                    throw new ArgumentException("Could not found user");
                using (var stream = new MemoryStream())
                {
                    await avatar.CopyToAsync(stream);
                    existing.Avatar = Convert.ToBase64String(stream.ToArray());
                    existing.LastActivity = DateTimeOffset.UtcNow;
                    _unitOfWork.Save();
                    return existing.Avatar;
                }
            }
            catch (ArgumentException)
            {
                throw;
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                throw new Exception("Error while getting user info. Please try a bit later");
            }
        }
    }
}

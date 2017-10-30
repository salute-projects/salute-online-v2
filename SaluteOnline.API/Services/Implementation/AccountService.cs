using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SaluteOnline.API.DAL;
using SaluteOnline.API.Providers.Interface;
using SaluteOnline.API.Services.Interface;
using SaluteOnline.Domain.Conversion;
using SaluteOnline.Domain.Domain.EF;
using SaluteOnline.Domain.Domain.Mongo;
using SaluteOnline.Domain.DTO;
using SaluteOnline.Domain.DTO.User;

namespace SaluteOnline.API.Services.Implementation
{
    public class AccountService : IAccountService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<AccountService> _logger;
        private readonly IAuthZeroProvider _authZeroProvider;
        private readonly IActivityService _activityService;

        public AccountService(IUnitOfWork unitOfWork, ILogger<AccountService> logger, IAuthZeroProvider authZeroProvider, IActivityService activityService)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _authZeroProvider = authZeroProvider;
            _activityService = activityService;
        }

        public async Task<SignUpResultDto> SignUp(UserEssential user)
        {
            try
            {
                var existing = _unitOfWork.Users.Count(t => t.Email == user.Email);
                if (existing != 0)
                    throw new ArgumentException("User already exists");
                var signUpResult = await _authZeroProvider.SignUp(user);
                if (signUpResult == null)
                    throw new ArgumentException("Auth0 signup process failed");
                var newUser = new User
                {
                    Email = user.Email,
                    Guid = Guid.NewGuid(),
                    Registered = DateTimeOffset.UtcNow,
                    LastActivity = DateTimeOffset.UtcNow,
                    Auth0Id = signUpResult.Id,
                    Role = Role.User
                };
                _unitOfWork.Users.Insert(newUser);
                await _unitOfWork.SaveAsync();
                var loginResult = await _authZeroProvider.GetToken(user);
                if (loginResult == null)
                    throw new ArgumentException("Auth0 login process failed");
                _activityService.LogActivity(new Activity
                {
                    UserId = newUser.Id,
                    Importance = ActivityImportance.Critical,
                    Type = ActivityType.SignUp,
                    Data = JsonConvert.SerializeObject(newUser)
                });
                return new SignUpResultDto
                {
                    Id = newUser.Id,
                    ExpiresIn = loginResult.ExpiresIn,
                    Token = loginResult.AccessToken,
                    RefreshToken = loginResult.RefreshToken
                };
            }
            catch (ArgumentException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        public bool UserExists(string email)
        {
            try
            {
                var exists = _unitOfWork.Users.Count(t => string.Equals(t.Email, email, StringComparison.OrdinalIgnoreCase));
                return exists != 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw new Exception("Error while getting user. Please try a bit later");
            }
        }

        public async Task<LoginResultDto> Login(UserEssential user)
        {
            try
            {
                var existing = _unitOfWork.Users.Get(t => t.Email == user.Email).FirstOrDefault();
                if (existing == null)
                    throw new ArgumentException("User doesn't exists");
                var loginResult = await _authZeroProvider.GetToken(user);
                if (loginResult == null)
                    throw new ArgumentException("Auth0 login process failed");
                _activityService.LogActivity(new Activity
                {
                    UserId = existing.Id,
                    Importance = ActivityImportance.Medium,
                    Type = ActivityType.SignUp
                });
                return new LoginResultDto
                {
                    ExpiresIn = loginResult.ExpiresIn,
                    Token = loginResult.AccessToken,
                    RefreshToken = loginResult.RefreshToken,
                    Avatar = existing.Avatar
                };
            }
            catch (ArgumentException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        public async Task<LoginResultDto> RefreshToken(string refreshToken)
        {
            try
            {
                var loginResult = await _authZeroProvider.RefreshToken(refreshToken);
                if (loginResult == null)
                    throw new ArgumentException("Auth0 login process failed");
                return new LoginResultDto
                {
                    ExpiresIn = loginResult.ExpiresIn,
                    Token = loginResult.AccessToken,
                    RefreshToken = loginResult.RefreshToken
                };
            }
            catch (ArgumentException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        public async Task<string> RunForgotPasswordFlow(string email)
        {
            try
            {
                if (string.IsNullOrEmpty(email))
                    throw new ArgumentException("Argument omitted");
                var existing = _unitOfWork.Users.Get(t => string.Equals(t.Email, email, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
                if (existing == null)
                    throw new ArgumentException("User with that email doesn't exists");
                var changePasswordResult = await _authZeroProvider.RunForgotPasswordFlow(email);
                _activityService.LogActivity(new Activity
                {
                    UserId = existing.Id,
                    Importance = ActivityImportance.High,
                    Type = ActivityType.SignUp
                });
                return changePasswordResult;
            }
            catch (ArgumentException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        public UserDto GetUserInfo(string email)
        {
            try
            {
                var user = _unitOfWork.Users.Get(t => t.Email.ToLower() == email).FirstOrDefault();
                if (user == null)
                    throw new ArgumentException("User not found");
                return user.ToDto();
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

        public void UpdateUserInfo(UserDto user, string email)
        {
            try
            {
                var existing = _unitOfWork.Users.GetById(id: user.Id);
                if (existing == null)
                    throw new ArgumentException("User does not exists");
                if (existing.Email != email)
                    throw new ArgumentException("Operation not allowed");
                user.UpdateEntity(existing);
                _activityService.LogActivity(new Activity
                {
                    UserId = existing.Id,
                    Importance = ActivityImportance.Medium,
                    Type = ActivityType.UserUpdate,
                    Data = JsonConvert.SerializeObject(existing)
                });
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

        public void UpdateMainUserInfo(UserMainInfoDto user, string email)
        {
            try
            {
                var existing = _unitOfWork.Users.GetById(id: user.Id);
                if (existing == null)
                    throw new ArgumentException("User does not exists");
                if (existing.Email != email)
                    throw new ArgumentException("Operation not allowed");
                user.UpdateEntity(existing);
                _activityService.LogActivity(new Activity
                {
                    UserId = existing.Id,
                    Importance = ActivityImportance.Medium,
                    Type = ActivityType.UserUpdate,
                    Data = JsonConvert.SerializeObject(existing)
                });
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

        public void UpdatePersonalUserInfo(UserPersonalInfoDto user, string email)
        {
            try
            {
                var existing = _unitOfWork.Users.GetById(id: user.Id);
                if (existing == null)
                    throw new ArgumentException("User does not exists");
                if (existing.Email != email)
                    throw new ArgumentException("Operation not allowed");
                user.UpdateEntity(existing);
                _activityService.LogActivity(new Activity
                {
                    UserId = existing.Id,
                    Importance = ActivityImportance.Medium,
                    Type = ActivityType.UserUpdate,
                    Data = JsonConvert.SerializeObject(existing)
                });
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

        public async Task<string> UpdateUserAvatar(IFormFile avatar, string email)
        {
            try
            {
                if (avatar == null || avatar.Length == 0 || string.IsNullOrEmpty(email))
                    throw new ArgumentException("File wasn't uploaded");
                var existing = _unitOfWork.Users.Get(t => t.Email == email).SingleOrDefault();
                if (existing == null)
                    throw new ArgumentException("Could not found user");
                using (var stream = new MemoryStream())
                {
                    await avatar.CopyToAsync(stream);
                    existing.Avatar = Convert.ToBase64String(stream.ToArray());
                    existing.LastActivity = DateTimeOffset.UtcNow;
                    _unitOfWork.Save();
                    _activityService.LogActivity(new Activity
                    {
                        UserId = existing.Id,
                        Importance = ActivityImportance.Medium,
                        Type = ActivityType.UserUpdate,
                        Data = JsonConvert.SerializeObject(existing)
                    });
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

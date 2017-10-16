using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SaluteOnline.API.DAL;
using SaluteOnline.API.DAL.Entities;
using SaluteOnline.API.Providers.Interface;
using SaluteOnline.API.Services.Interface;
using SaluteOnline.Domain.DTO.User;

namespace SaluteOnline.API.Services.Implementation
{
    public class AccountService : IAccountService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<AccountService> _logger;
        private readonly IAuthZeroProvider _authZeroProvider;

        public AccountService(IUnitOfWork unitOfWork, ILogger<AccountService> logger, IAuthZeroProvider authZeroProvider)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _authZeroProvider = authZeroProvider;
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
                    Auth0Id = signUpResult.Id
                };
                _unitOfWork.Users.Insert(newUser);
                await _unitOfWork.SaveAsync();
                var loginResult = await _authZeroProvider.GetToken(user);
                if (loginResult == null)
                    throw new ArgumentException("Auth0 login process failed");
                return new SignUpResultDto
                {
                    Id = newUser.Id,
                    ExpiresIn = loginResult.ExpiresIn,
                    Token = loginResult.AccessToken
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
                var existing = _unitOfWork.Users.Count(t => t.Email == user.Email);
                if (existing == 0)
                    throw new ArgumentException("User doesn't exists");
                var loginResult = await _authZeroProvider.GetToken(user);
                if (loginResult == null)
                    throw new ArgumentException("Auth0 login process failed");
                return new LoginResultDto
                {
                    ExpiresIn = loginResult.ExpiresIn,
                    Token = loginResult.AccessToken
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
    }
}

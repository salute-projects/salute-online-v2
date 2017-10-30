using System.Threading.Tasks;
using Auth0.Core;
using SaluteOnline.Domain.DTO.Auth0;
using SaluteOnline.Domain.DTO.User;

namespace SaluteOnline.API.Providers.Interface
{
    public interface IAuthZeroProvider
    {
        Task<SignUpResponse> SignUp(UserEssential user);
        Task<LoginResponse> GetToken(UserEssential user);
        Task<LoginResponse> RefreshToken(string refreshToken);
        Task<string> RunForgotPasswordFlow(string email);
        Task<User> GetUserById(string userId);
        Task<User> GetUserByEmail(string email);
        Task<User> GetUserByToken(string token);
    }
}
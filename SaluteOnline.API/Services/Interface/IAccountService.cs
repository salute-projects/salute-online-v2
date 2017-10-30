using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using SaluteOnline.Domain.DTO.User;

namespace SaluteOnline.API.Services.Interface
{
    public interface IAccountService
    {
        bool UserExists(string email);
        Task<SignUpResultDto> SignUp(UserEssential user);
        Task<LoginResultDto> Login(UserEssential user);
        Task<LoginResultDto> RefreshToken(string refreshToken);
        Task<string> RunForgotPasswordFlow(string email);

        UserDto GetUserInfo(string email);
        void UpdateUserInfo(UserDto user, string email);
        void UpdateMainUserInfo(UserMainInfoDto user, string email);
        void UpdatePersonalUserInfo(UserPersonalInfoDto user, string email);
        Task<string> UpdateUserAvatar(IFormFile avatar, string email);
    }
}
using System.Threading.Tasks;
using SaluteOnline.Domain.DTO.User;

namespace SaluteOnline.API.Services.Interface
{
    public interface IAccountService
    {
        bool UserExists(string email);
        Task<SignUpResultDto> SignUp(UserEssential user);
        Task<LoginResultDto> Login(UserEssential user);
        Task<string> RunForgotPasswordFlow(string email);

        UserDto GetUserInfo(string email);
    }
}
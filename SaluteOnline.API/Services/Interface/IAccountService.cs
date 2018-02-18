using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using SaluteOnline.API.DTO.User;

namespace SaluteOnline.API.Services.Interface
{
    public interface IAccountService
    {
        UserDto GetUserInfo(string email);
        void UpdateUserInfo(UserDto user, string email);
        void UpdateMainUserInfo(UserMainInfoDto user, string email);
        void UpdatePersonalUserInfo(UserPersonalInfoDto user, string email);
        Task<string> UpdateUserAvatar(IFormFile avatar, string email);
    }
}
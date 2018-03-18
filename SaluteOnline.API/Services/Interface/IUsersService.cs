using SaluteOnline.API.DTO.User;
using SaluteOnline.Shared.Common;

namespace SaluteOnline.API.Services.Interface
{
    public interface IUsersService
    {
        Page<UserDto> GetUsers(UserFilter request);
        void SetUserRole(SetRoleRequest request);
        void SetUserStatus(SetStatusRequest request);
    }
}
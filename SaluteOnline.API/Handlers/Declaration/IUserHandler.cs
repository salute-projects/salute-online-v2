using SaluteOnline.Domain.DTO.Activity;

namespace SaluteOnline.API.Handlers.Declaration
{
    public interface IUserHandler
    {
        bool HandleNewUser(UserRegisteredEvent data);
    }
}
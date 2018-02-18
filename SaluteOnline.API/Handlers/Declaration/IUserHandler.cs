using SaluteOnline.Shared.Events;

namespace SaluteOnline.API.Handlers.Declaration
{
    public interface IUserHandler
    {
        bool HandleNewUser(UserRegisteredEvent data);
    }
}
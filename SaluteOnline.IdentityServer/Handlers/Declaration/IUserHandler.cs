using System.Threading.Tasks;
using SaluteOnline.Shared.Events;

namespace SaluteOnline.IdentityServer.Handlers.Declaration
{
    public interface IUserHandler
    {
        Task<bool> HandleUserCreated(UserCreatedEvent data);
        Task<bool> HandleUserRoleChanged(UserRoleChangeEvent data);
    }
}
using System.Threading.Tasks;
using SaluteOnline.Domain.DTO.Activity;

namespace SaluteOnline.IdentityServer.Handlers.Declaration
{
    public interface IUserHandler
    {
        Task<bool> HandleUserCreated(UserCreatedEvent data);
    }
}
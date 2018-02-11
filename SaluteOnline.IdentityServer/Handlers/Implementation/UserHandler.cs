using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using SaluteOnline.Domain.DTO.Activity;
using SaluteOnline.IdentityServer.Domain;
using SaluteOnline.IdentityServer.Handlers.Declaration;

namespace SaluteOnline.IdentityServer.Handlers.Implementation
{
    public class UserHandler : IUserHandler
    {
        private readonly UserManager<SoApplicationUser> _userManager;

        public UserHandler(UserManager<SoApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<bool> HandleUserCreated(UserCreatedEvent data)
        {
            var user = await _userManager.FindByIdAsync(data.SubjectId);
            user.UserId = data.UserId;
            await _userManager.UpdateAsync(user);
            return true;
        }
    }
}

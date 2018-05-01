using System.Threading.Tasks;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using SaluteOnline.IdentityServer.Domain;
using SaluteOnline.IdentityServer.Handlers.Declaration;
using SaluteOnline.Shared.Events;

namespace SaluteOnline.IdentityServer.Handlers.Implementation
{
    public class UserHandler : IUserHandler
    {
        private readonly UserManager<SoApplicationUser> _userManager;
        private readonly IRefreshTokenStore _refreshTokenStore;
        private readonly WebApplicationClientSettings _webApplicationClientSettings;

        public UserHandler(UserManager<SoApplicationUser> userManager, IRefreshTokenStore refreshTokenStore, IOptions<WebApplicationClientSettings> webAppSettings)
        {
            _userManager = userManager;
            _refreshTokenStore = refreshTokenStore;
            _webApplicationClientSettings = webAppSettings.Value;
        }

        public async ValueTask<bool> HandleUserCreated(UserCreated data)
        {
            var user = await _userManager.FindByIdAsync(data.SubjectId);
            user.UserId = data.UserId;
            await _userManager.UpdateAsync(user);
            return true;
        }

        public async ValueTask<bool> HandleUserRoleChanged(UserRoleChangeEvent data)
        {
            var user = await _userManager.FindByIdAsync(data.SubjectId);
            user.Role = data.Role;
            await _userManager.UpdateAsync(user);
            await _refreshTokenStore.RemoveRefreshTokensAsync(data.SubjectId, _webApplicationClientSettings.ClientId);
            return true;
        }
    }
}

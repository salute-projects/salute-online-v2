using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Identity;
using SaluteOnline.IdentityServer.Domain;

namespace SaluteOnline.IdentityServer.Service.Implementation
{
    public class SoProfileService : IProfileService
    {
        private readonly UserManager<SoApplicationUser> _userManager;
        private readonly IUserClaimsPrincipalFactory<SoApplicationUser> _claimsFactory;

        public SoProfileService(UserManager<SoApplicationUser> userManager, IUserClaimsPrincipalFactory<SoApplicationUser> claimsFactory)
        {
            _userManager = userManager;
            _claimsFactory = claimsFactory;
        }

        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var sub = context.Subject.GetSubjectId();
            var user = await _userManager.FindByIdAsync(sub);
            var principal = await _claimsFactory.CreateAsync(user);
            var claims = principal.Claims.Where(t => context.RequestedClaimTypes.Contains(t.Type)).ToList();
            claims.Add(new Claim(JwtClaimTypes.Role, user.Role.ToString()));
            claims.Add(new Claim(JwtClaimTypes.Picture, user.AvatarUrl ?? string.Empty));
            claims.Add(new Claim("userId", user.UserId.ToString()));
            context.IssuedClaims = claims;
        }

        public async Task IsActiveAsync(IsActiveContext context)
        {
            var sub = context.Subject.GetSubjectId();
            var user = await _userManager.FindByIdAsync(sub);
            context.IsActive = user != null;
        }
    }
}

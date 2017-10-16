using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace SaluteOnline.API.Security
{
    public class ScoreRequirement : AuthorizationHandler<ScoreRequirement>, IAuthorizationRequirement
    {
        private readonly string _issuer;
        private readonly string _scope;

        public ScoreRequirement(string issuer, string scope)
        {
            _scope = scope;
            _issuer = issuer;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ScoreRequirement requirement)
        {
            //if (!context.User.HasClaim(t => t.Type == "scope" && t.Issuer == _issuer))
            //    return Task.CompletedTask;
            //var scopes = context.User.FindFirst(t => t.Type == "scope" && t.Issuer == _issuer).Value.Split(" ");
            //if (scopes.Any(t => t == _scope))
            //    context.Succeed(requirement);
            context.Succeed(requirement);
            return Task.CompletedTask;
        }
    }
}

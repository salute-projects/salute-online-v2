using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using Auth0.Core;
using Microsoft.Extensions.Caching.Memory;
using SaluteOnline.API.Providers.Interface;

namespace SaluteOnline.API.Security
{
    public class CustomAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly IMemoryCache _memoryCache;
        private readonly IAuthZeroProvider _authZeroProvider;

        public CustomAuthHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger,
            System.Text.Encodings.Web.UrlEncoder encoder, ISystemClock clock, IMemoryCache memoryCache, IAuthZeroProvider authZeroProvider) :
            base(options, logger, encoder, clock)
        {
            _memoryCache = memoryCache;
            _authZeroProvider = authZeroProvider;
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.TryGetValue(HeaderNames.Authorization, out var authorization))
            {
                return Task.FromResult(AuthenticateResult.Fail("Missing or malformed Authorization header."));
            }
            var token = authorization.First();
            var user = GetUser(token, true);
            if (!user.EmailVerified.HasValue || !user.EmailVerified.Value)
                return Task.FromResult(AuthenticateResult.Fail("Users email is not verified."));
            var identity = new ClaimsIdentity("Auth");
            var metadata = user.AppMetadata;
            if (metadata != null)
            {
                var role = user.AppMetadata["role"]?.ToString();
                if (string.IsNullOrEmpty(role))
                    return Task.FromResult(AuthenticateResult.Fail("Missing user role."));
                identity.AddClaim(new Claim("role", role));
            }
            else
            {
                _memoryCache.Remove(token);
            }
            identity.AddClaim(new Claim("authUserId", user.UserId));
            identity.AddClaim(new Claim("email", user.Email));
            identity.AddClaim(new Claim("token", token));
            identity.AddClaim(new Claim("avatar", user.Picture));
            var ticket = new AuthenticationTicket(new ClaimsPrincipal(identity), null, "Auth");
            return Task.FromResult(AuthenticateResult.Success(ticket));
        }

        private User GetUser(string token, bool tryReadFromCache)
        {
            User user = null;
            var cacheExists = false;
            if (tryReadFromCache)
            {
                cacheExists = _memoryCache.TryGetValue(token, out user);
            }
            if (cacheExists)
                return user;
            user = _authZeroProvider.GetUserByToken(token).Result;
            _memoryCache.Set(token, user, new MemoryCacheEntryOptions { SlidingExpiration = TimeSpan.FromMinutes(5)});
            return user;
        }
    }
}

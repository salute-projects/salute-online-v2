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

            var identities = new List<ClaimsIdentity> { new ClaimsIdentity("custom auth type") };
            var ticket = new AuthenticationTicket(new ClaimsPrincipal(identities), "Auth");
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

using System;
using Microsoft.AspNetCore.Authentication;

namespace SaluteOnline.API.Security
{
    public static class AuthenticationBuilderExtensions
    {
        public static AuthenticationBuilder AddCustomAuth(this AuthenticationBuilder builder, Action<AuthenticationSchemeOptions> configureOptions)
        {
            return builder.AddScheme<AuthenticationSchemeOptions, CustomAuthHandler>("Auth", configureOptions);
        }
    }
}

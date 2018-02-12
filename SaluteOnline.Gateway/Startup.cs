using System;
using System.Linq;
using System.Security.Claims;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using SaluteOnline.Domain.DTO;
using ConfigurationBuilder = Microsoft.Extensions.Configuration.ConfigurationBuilder;

namespace SaluteOnline.Gateway
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder();
            builder.SetBasePath(env.ContentRootPath)
                .AddJsonFile("ocelot_configuration.json", false, true)
                .AddJsonFile("appsettings.json", false, true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication().AddIdentityServerAuthentication("safeKey", options =>
            {
                options.Authority = Configuration.GetSection("IdentityServerSettings").GetValue<string>("Authority");
                options.RequireHttpsMetadata =
                    Configuration.GetSection("IdentityServerSettings").GetValue<bool>("RequireHttpsMetadata");
                options.ApiName = Configuration.GetSection("IdentityServerSettings").GetValue<string>("ApiName");
                options.SupportedTokens = SupportedTokens.Both;
            });

            services.AddCors(
                options =>
                {
                    options.AddPolicy("CorsPolicy",
                        builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader().AllowCredentials());
                });
            services.AddOcelot(Configuration);
        }

        public async void Configure(IApplicationBuilder app, IHostingEnvironment env, IMemoryCache memoryCache)
        {
            app.UseCors("CorsPolicy");
            await app.UseOcelot(new OcelotMiddlewareConfiguration
            {
                PreAuthorisationMiddleware = async (context, func) =>
                {
                    var role = context.User.Claims.ToList().FirstOrDefault(t => t.Type.ToLower() == "role");
                    if (!string.IsNullOrEmpty(role?.Value))
                    {
                        var transformedRole = TransformRoleClaim(role.Value);
                        if (transformedRole == "corrupted")
                            return;
                        var newIdentity = new ClaimsIdentity("Auth");
                        newIdentity.AddClaim(new Claim("role", transformedRole));
                        context.User = new ClaimsPrincipal(newIdentity);
                        await func.Invoke();
                    }
                }
            });
        }

        private static string TransformRoleClaim(string role)
        {
            Enum.TryParse<Roles>(role, out var roleValue);
            switch (roleValue)
            {
                case Roles.SilentDon:
                    return "sd";
                case Roles.GlobalAdmin:
                    return "sd_ga";
                case Roles.ClubAdmin:
                    return "sd_ga_ca";
                case Roles.User:
                    return "sd_ga_ca_user";
                case Roles.Guest:
                    return "all";
                default:
                    return "corrupted";
            };
        }
    }
}

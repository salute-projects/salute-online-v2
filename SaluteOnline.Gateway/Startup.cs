using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using ConfigurationBuilder = Microsoft.Extensions.Configuration.ConfigurationBuilder;

namespace SaluteOnline.Gateway
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder();
            builder.SetBasePath(env.ContentRootPath).AddJsonFile("ocelot_configuration.json", false, true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = "Auth";
                options.DefaultChallengeScheme = "Auth";
            }).AddJwtBearer("authKey", options =>
            {
                options.Audience = "https://saluteonline.eu.auth0.com/api/v2/";
                options.Authority = "https://saluteonline.eu.auth0.com/";
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
                    var scopes = context.User.Claims.ToList().FirstOrDefault(t => t.Type == "scope");
                    if (scopes != null && scopes.Value.Contains("silentdon"))
                    {
                        var newIdentity = new ClaimsIdentity("Auth");
                        newIdentity.AddClaim(new Claim("role", "silentdon"));
                        context.User.AddIdentity(newIdentity);
                        await func.Invoke();
                    }
                }
            });
        }
    }
}

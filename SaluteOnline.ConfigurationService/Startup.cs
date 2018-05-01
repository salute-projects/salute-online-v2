using System.Collections.Generic;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SaluteOnline.ConfigurationService.DAL;
using SaluteOnline.ConfigurationService.Service.Declaration;

namespace SaluteOnline.ConfigurationService
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            var authSettings = Configuration.GetSection("Auth");

            services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
                .AddIdentityServerAuthentication(options =>
            {
                options.Authority = authSettings["Domain"];
                options.ApiName = "salute_security_api";
                options.RequireHttpsMetadata = false;
            });

            services.AddCors(
                options =>
                {
                    options.AddPolicy("CorsPolicy",
                        builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader().AllowCredentials());
                });

            services.AddSingleton(Configuration);
            services.AddMvc(options =>
            {
                options.CacheProfiles.Add(new KeyValuePair<string, CacheProfile>("CachingProfile", new CacheProfile
                {
                    Duration = 360,
                    Location = ResponseCacheLocation.Any,
                    VaryByHeader = "user-agent"
                }));
            }).AddJsonOptions(jsonOptions =>
            {
                jsonOptions.SerializerSettings.DefaultValueHandling = DefaultValueHandling.Populate;
                jsonOptions.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                jsonOptions.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                jsonOptions.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            });

            services.AddSingleton(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddSingleton<IConfigurationService, Service.Implementation.ConfigurationService>();
            services.AddMemoryCache();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseAuthentication();
            app.UseMvc();
            SeedDefault.Seed(Configuration.GetValue<string>("MongoSettings:Path"), Configuration.GetValue<string>("MongoSettings:DB"));
        }
    }
}

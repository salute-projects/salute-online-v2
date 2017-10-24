using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Serialization;
using SaluteOnline.API.DAL;
using SaluteOnline.API.Providers.Implementation;
using SaluteOnline.API.Providers.Interface;
using SaluteOnline.API.Security;
using SaluteOnline.API.Services.Implementation;
using SaluteOnline.API.Services.Interface;

namespace SaluteOnline.API
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
            services.Configure<Auth0Settings>(Configuration.GetSection("Auth0"));
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = "Auth";
                options.DefaultChallengeScheme = "Auth";
            }).AddCustomAuth(options => {});
            services.AddAuthorization(options =>
            {
                options.AddPolicy("Auth", policy =>
                {
                    policy.RequireClaim("id_token");
                });
            });
            var connectionString = Configuration.GetConnectionString("SoConnection");
            services.AddDbContext<SaluteOnlineDbContext>(options => options.UseSqlServer(connectionString));
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddSingleton(Configuration);

            services.AddCors(
                options =>
                {
                    options.AddPolicy("CorsPolicy",
                        builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader().AllowCredentials());
                });

            services.AddMvc().AddJsonOptions(jsonOptions =>
            {
                jsonOptions.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
                jsonOptions.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            });

            InitializeSettings(services);
            InitializeProviders(services);
            InitializeServices(services);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseAuthentication();
            app.UseCors("CorsPolicy");
            app.UseMvc();
        }

        private static void InitializeServices(IServiceCollection services)
        {
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IActivityService, ActivityService>();
        }

        private static void InitializeProviders(IServiceCollection services)
        {
            services.AddScoped<IAuthZeroProvider, AuthZeroProvider>();
        }

        private void InitializeSettings(IServiceCollection services)
        {
            services.Configure<Auth0Settings>(Configuration.GetSection("Auth0"));
        }
    }
}

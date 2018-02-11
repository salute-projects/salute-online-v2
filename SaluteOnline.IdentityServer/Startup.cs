using System.Collections.Generic;
using System.Threading.Tasks;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using RawRabbit;
using RawRabbit.Configuration;
using RawRabbit.DependencyInjection.ServiceCollection;
using RawRabbit.Instantiation;
using SaluteOnline.Domain.DTO.Activity;
using SaluteOnline.IdentityServer.Constants;
using SaluteOnline.IdentityServer.DAL;
using SaluteOnline.IdentityServer.Domain;
using SaluteOnline.IdentityServer.Handlers.Declaration;
using SaluteOnline.IdentityServer.Handlers.Implementation;
using SaluteOnline.IdentityServer.Service.Declaration;
using SaluteOnline.IdentityServer.Service.Implementation;

namespace SaluteOnline.IdentityServer
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
            services.AddDbContext<IsDbContext>(
                options => options.UseSqlServer(Configuration.GetConnectionString("IdentityDb")));

            services.AddIdentity<SoApplicationUser, IdentityRole>(config =>
                {
                    config.SignIn.RequireConfirmedEmail = true;
                    config.SignIn.RequireConfirmedPhoneNumber = false;
                    config.Lockout.MaxFailedAccessAttempts = 10;
                    config.Password.RequireDigit = false;
                    config.Password.RequireLowercase = false;
                    config.Password.RequireNonAlphanumeric = false;
                    config.Password.RequiredUniqueChars = 0;
                    config.Password.RequireUppercase = false;
                    config.Password.RequiredLength = 6;
                })
                .AddDefaultTokenProviders()
                .AddEntityFrameworkStores<IsDbContext>();

            services.AddSingleton((IConfigurationRoot) Configuration);
            services.AddTransient<IProfileService, SoProfileService>();
            services.AddSingleton<IUserHandler, UserHandler>();
            services.AddSingleton<IBusService, BusService>();

            services.Configure<WebApplicationClientSettings>(Configuration.GetSection("WebApplicationClientSettings"));
            services.Configure<ResourceOwnerClientSettings>(Configuration.GetSection("ResourceOwnerClientSettings"));
            services.Configure<ApiClientSettings>(Configuration.GetSection("ApiClientSettings"));

            services.AddCors(
                options =>
                {
                    options.AddPolicy("CorsPolicy",
                        builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader().AllowCredentials());
                });

            services.AddIdentityServer()
                .AddDeveloperSigningCredential(filename: "tempkey.rsa")
                .AddInMemoryApiResources(DefaultInfrastructure.GetApiResources(Configuration.GetSection("ApiClientSettings")))
                .AddInMemoryIdentityResources(DefaultInfrastructure.GetIdentityResources())
                .AddInMemoryClients(DefaultInfrastructure.GetClients(Configuration.GetSection("WebApplicationClientSettings"), Configuration.GetSection("ResourceOwnerClientSettings")))
                .AddAspNetIdentity<SoApplicationUser>()
                .AddProfileService<SoProfileService>();

            services.AddRawRabbit(GetRabbitConfiguration);

            SubscribeToRabbit(services);

            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseCors("CorsPolicy");
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseIdentityServer();
            app.UseStaticFiles();
            app.UseMvcWithDefaultRoute();
        }

        private static RawRabbitOptions GetRabbitConfiguration => new RawRabbitOptions
        {
            ClientConfiguration = new RawRabbitConfiguration
            {
                Username = "guest",
                Password = "guest",
                VirtualHost = "/",
                Port = 32770,
                Hostnames = new List<string> { "127.0.0.1" }
            }
        };

        private static async void SubscribeToRabbit(IServiceCollection services)
        {
            var bus = RawRabbitFactory.CreateSingleton(GetRabbitConfiguration);
            var handler = services.BuildServiceProvider().GetService<IUserHandler>();
            await bus.SubscribeAsync<UserCreatedEvent>(msg => Task.FromResult(handler.HandleUserCreated(msg)));
        }
    }
}

using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using RawRabbit;
using RawRabbit.Configuration;
using RawRabbit.DependencyInjection.ServiceCollection;
using RawRabbit.Instantiation;
using SaluteOnline.API.DAL;
using SaluteOnline.API.Handlers.Declaration;
using SaluteOnline.API.Handlers.Implementation;
using SaluteOnline.API.Hub;
using SaluteOnline.API.Security;
using SaluteOnline.API.Services.Implementation;
using SaluteOnline.API.Services.Interface;
using SaluteOnline.Domain.DTO;
using SaluteOnline.Domain.DTO.Activity;
using Swashbuckle.AspNetCore.Swagger;

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
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = "Auth";
                options.DefaultChallengeScheme = "Auth";
            }).AddCustomAuth(options => {});
            services.AddAuthorization(options =>
            {
                options.AddPolicy("Auth", policy =>
                {
                    policy.RequireClaim("subjectId");
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

            services.AddSignalR();

            services.AddSwaggerGen(t =>
            {
                t.SwaggerDoc("v1", new Info
                {
                    Version = "v1",
                    Title = "Salute Online API"
                });
            });
            services.ConfigureSwaggerGen(t =>
            {
                t.AddSecurityDefinition("Bearer", new ApiKeyScheme
                {
                    In = "header",
                    Description = "Bearer",
                    Name = "Authorization",
                    Type = "apiKey"
                });
            });

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

            services.AddRawRabbit(GetRabbitConfiguration);

            InitializeSettings(services);
            InitializeServices(services);

            SetPolicies(services);

            SubsribeToRabbit(services);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseAuthentication();
            app.UseCors("CorsPolicy");
            app.UseSwagger();
            app.UseSwaggerUI(t =>
            {
                t.SwaggerEndpoint("/swagger/v1/swagger.json", "Salute Online API");
            });
            app.UseSignalR(routes =>
            {
                routes.MapHub<SoMessageHub>("soMessageHub");
            });
            app.UseMvc();
        }

        private static void InitializeServices(IServiceCollection services)
        {
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<ICommonService, CommonService>();
            services.AddScoped<IClubsService, ClubsService>();
            services.AddScoped<IChatService, ChatService>();
            services.AddSingleton<IBusService, BusService>();
            services.AddScoped<IUserHandler, UserHandler>();
        }

        private void InitializeSettings(IServiceCollection services)
        {
            services.Configure<AuthSettings>(Configuration.GetSection("Auth"));
        }

        private static void SetPolicies(IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                options.AddPolicy(Policies.User.ToString(),
                    policyUser => policyUser.RequireClaim("role", Roles.User.ToString(), Roles.ClubAdmin.ToString(), Roles.GlobalAdmin.ToString(), Roles.SilentDon.ToString()));
                options.AddPolicy(Policies.ClubAdmin.ToString(),
                    policyUser => policyUser.RequireClaim("role", Roles.ClubAdmin.ToString(), Roles.GlobalAdmin.ToString(), Roles.SilentDon.ToString()));
                options.AddPolicy(Policies.GlobalAdmin.ToString(),
                    policyUser => policyUser.RequireClaim("role", Roles.GlobalAdmin.ToString(), Roles.SilentDon.ToString()));
                options.AddPolicy(Policies.SilendDon.ToString(),
                    policyUser => policyUser.RequireClaim("role", Roles.SilentDon.ToString()));
            });
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

        private static async void SubsribeToRabbit(IServiceCollection services)
        {
            var bus = RawRabbitFactory.CreateSingleton(GetRabbitConfiguration);
            var handler = services.BuildServiceProvider().GetService<IUserHandler>();
            await bus.SubscribeAsync<UserRegisteredEvent>(msg => Task.FromResult(handler.HandleNewUser(msg)));
        }
    }
}

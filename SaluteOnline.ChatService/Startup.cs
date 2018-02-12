using Mapster;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SaluteOnline.ChatService.DAL;
using SaluteOnline.ChatService.Domain;
using SaluteOnline.ChatService.Domain.DTO;
using SaluteOnline.ChatService.Security;
using SaluteOnline.ChatService.Service.Abstraction;
using SaluteOnline.Domain.DTO;
using Swashbuckle.AspNetCore.Swagger;

namespace SaluteOnline.ChatService
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
            }).AddCustomAuth(options => { });
            services.AddAuthorization(options =>
            {
                options.AddPolicy("Auth", policy =>
                {
                    policy.RequireClaim("subjectId");
                });
            });

            services.AddSingleton(Configuration);

            services.AddCors(
                options =>
                {
                    options.AddPolicy("CorsPolicy",
                        builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader().AllowCredentials());
                });

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

            RegisterServices(services);
            RegisterMapsterProfiles();
            services.AddMvc().AddJsonOptions(jsonOptions =>
            {
                jsonOptions.SerializerSettings.DefaultValueHandling = DefaultValueHandling.Populate;
                jsonOptions.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                jsonOptions.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                jsonOptions.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            });

            SetPolicies(services);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
        }

        private void RegisterServices(IServiceCollection services)
        {
            services.AddSingleton(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddSingleton<IChatService, Service.Implementation.ChatService>();
            services.Configure<AuthSettings>(Configuration.GetSection("Auth"));
        }

        private static void RegisterMapsterProfiles()
        {
            TypeAdapterConfig<Chat, ChatDto>.NewConfig().Ignore(t => t.Participants);
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
    }
}

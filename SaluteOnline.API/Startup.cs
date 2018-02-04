using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using RawRabbit.Configuration;
using RawRabbit.DependencyInjection.ServiceCollection;
using RawRabbit.Instantiation;
using SaluteOnline.API.DAL;
using SaluteOnline.API.Hub;
using SaluteOnline.API.Providers.Implementation;
using SaluteOnline.API.Providers.Interface;
using SaluteOnline.API.Security;
using SaluteOnline.API.Services.Implementation;
using SaluteOnline.API.Services.Interface;
using SaluteOnline.Domain.DTO;
using SaluteOnline.Domain.Extensions;
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
                    Description = "Bearer eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsImtpZCI6IlJqY3hPVFF6TXpJd01EUkZNVU13TkVaQk5qTkZSa0pHTVRnNFFVTkdSRVExTXpoRE1UTkVNZyJ9.eyJpc3MiOiJodHRwczovL3NhbHV0ZW9ubGluZS5ldS5hdXRoMC5jb20vIiwic3ViIjoiYXV0aDB8NTllOTllYzcyM2MyZTUyNGZmNzU1ZTBiIiwiYXVkIjoiaHR0cHM6Ly9zYWx1dGVvbmxpbmUuZXUuYXV0aDAuY29tL2FwaS92Mi8iLCJpYXQiOjE1MTUzNDA2NzksImV4cCI6MTUxNTQyNzA3OSwiYXpwIjoiM3MtTURveWJlOWI2akRiTXE2anZaQWhNc1JGaEhURTciLCJzY29wZSI6InJlYWQ6Y3VycmVudF91c2VyIHVwZGF0ZTpjdXJyZW50X3VzZXJfbWV0YWRhdGEgZGVsZXRlOmN1cnJlbnRfdXNlcl9tZXRhZGF0YSBjcmVhdGU6Y3VycmVudF91c2VyX21ldGFkYXRhIGNyZWF0ZTpjdXJyZW50X3VzZXJfZGV2aWNlX2NyZWRlbnRpYWxzIGRlbGV0ZTpjdXJyZW50X3VzZXJfZGV2aWNlX2NyZWRlbnRpYWxzIHVwZGF0ZTpjdXJyZW50X3VzZXJfaWRlbnRpdGllcyBvZmZsaW5lX2FjY2VzcyIsImd0eSI6InBhc3N3b3JkIn0.soPA-T4NZnLrKrAU2zFIsHnN1wnkOvNxU3V2Y6c_jMC-mWI89bRigVyV0TYOnK8WYjWC2yvmi5oyUejl3T-1-cASRK03Bw1yFfGJVERBmmwP3MUU4hEOvrp_dtLmVVhIuxam7R0r8WrAcnxNrlv6wh2QcRdnCNsRQHdihIphKnNpI6lhb7JSApvourAGU0-e2qdV8T2zoV_jsuvN83i7CPSa7iLQvcGyTUlbOR7UytbrwTGSpN4HJ2Qf3oED55DWFj31c6tJQn0esbgsRfsPcqHgI6Fj_5vn94Ghkx6CQ5gG3UUBmktySG3vlSfvZcOEG3M06L-iAbkfcKXHk5R07A",
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
            InitializeProviders(services);
            InitializeServices(services);

            SetPolicies(services);
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
        }

        private static void InitializeProviders(IServiceCollection services)
        {
            services.AddScoped<IAuthZeroProvider, AuthZeroProvider>();
        }

        private void InitializeSettings(IServiceCollection services)
        {
            services.Configure<Auth0Settings>(Configuration.GetSection("Auth0"));
        }

        private static void SetPolicies(IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                options.AddPolicy(Policies.User.ToString(),
                    policyUser => policyUser.RequireClaim("role", Roles.User.ToLowerString(), Roles.ClubAdmin.ToLowerString(), Roles.GlobalAdmin.ToLowerString(), Roles.SilentDon.ToLowerString()));
                options.AddPolicy(Policies.ClubAdmin.ToString(),
                    policyUser => policyUser.RequireClaim("role", Roles.ClubAdmin.ToLowerString(), Roles.GlobalAdmin.ToLowerString(), Roles.SilentDon.ToLowerString()));
                options.AddPolicy(Policies.GlobalAdmin.ToString(),
                    policyUser => policyUser.RequireClaim("role", Roles.GlobalAdmin.ToLowerString(), Roles.SilentDon.ToLowerString()));
                options.AddPolicy(Policies.SilendDon.ToString(),
                    policyUser => policyUser.RequireClaim("role", Roles.SilentDon.ToLowerString()));
            });
        }

        private static RawRabbitOptions GetRabbitConfiguration => new RawRabbitOptions
        {
            ClientConfiguration = new RawRabbitConfiguration
            {
                Username = "guest",
                Password = "guest",
                VirtualHost = "/",
                Port = 32775,
                Hostnames = new List<string> { "127.0.0.1" }
            }
        };
    }
}

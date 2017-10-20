using System.Linq;
using Microsoft.AspNetCore.Authentication.JwtBearer;
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

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var domain = Configuration.GetSection("Auth0").GetChildren().First(t => t.Key == "Domain").Value;
            services.Configure<Auth0Settings>(Configuration.GetSection("Auth0"));
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.Authority = Configuration.GetSection("Auth0").GetChildren().First(t => t.Key == "Domain").Value;
                options.Audience = Configuration["Auth0:ApiIdentifier"];
            });
            services.AddAuthorization(options =>
            {
                options.AddPolicy("read:all",
                    policy => policy.Requirements.Add(new ScoreRequirement("read:all", domain)));
                options.AddPolicy("write:all",
                    policy => policy.Requirements.Add(new ScoreRequirement("write:all", domain)));
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

            services.AddScoped<IAccountService, AccountService>();

            // settings
            services.Configure<Auth0Settings>(Configuration.GetSection("Auth0"));

            //providers
            services.AddScoped<IAuthZeroProvider, AuthZeroProvider>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
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
    }
}

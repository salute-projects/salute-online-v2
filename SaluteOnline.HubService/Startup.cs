using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using RawRabbit;
using RawRabbit.Configuration;
using RawRabbit.DependencyInjection.ServiceCollection;
using RawRabbit.Instantiation;
using SaluteOnline.HubService.DAL;
using SaluteOnline.HubService.Handlers.Declaration;
using SaluteOnline.HubService.Hubs;
using SaluteOnline.HubService.Security;
using SaluteOnline.Shared.Events;

namespace SaluteOnline.HubService
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

            services.AddCors(
               options =>
               {
                   options.AddPolicy("CorsPolicy",
                       builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader().AllowCredentials());
               });

            services.AddSignalR();
            services.AddMvc();
            services.AddRawRabbit(GetRabbitConfiguration);
            services.AddSingleton(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.Configure<AuthSettings>(Configuration.GetSection("Auth"));
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
            app.UseSignalR(routes =>
            {
                routes.MapHub<SoMessageHub>("soMessageHub");
            });
            app.UseMvc();
        }

        private const string RabbitSectionName = "RabbitSettings";
        private RawRabbitOptions GetRabbitConfiguration => new RawRabbitOptions
        {
            ClientConfiguration = new RawRabbitConfiguration
            {
                Username = Configuration.GetSection(RabbitSectionName).GetValue<string>(nameof(RawRabbitConfiguration.Username)),
                Password = Configuration.GetSection(RabbitSectionName).GetValue<string>(nameof(RawRabbitConfiguration.Password)),
                VirtualHost = Configuration.GetSection(RabbitSectionName).GetValue<string>(nameof(RawRabbitConfiguration.VirtualHost)),
                Port = Configuration.GetSection(RabbitSectionName).GetValue<int>(nameof(RawRabbitConfiguration.Port)),
                Hostnames = Configuration.GetSection(RabbitSectionName).GetSection(nameof(RawRabbitConfiguration.Hostnames)).Get<List<string>>(),
                Ssl = new SslOption
                {
                    Enabled = Configuration.GetSection(RabbitSectionName).GetValue<bool>("SslEnabled")
                },
                AutomaticRecovery = Configuration.GetSection(RabbitSectionName).GetValue<bool>(nameof(RawRabbitConfiguration.AutomaticRecovery)),
                RecoveryInterval = TimeSpan.FromSeconds(Configuration.GetSection(RabbitSectionName).GetValue<int>(nameof(RawRabbitConfiguration.RecoveryInterval)))
            }
        };

        private async void SubsribeToRabbit(IServiceCollection services)
        {
            var bus = RawRabbitFactory.CreateSingleton(GetRabbitConfiguration);
            var handler = services.BuildServiceProvider().GetService<IHubHandler>();
            await bus.SubscribeAsync<HubEvent>(msg => Task.FromResult(handler.HandleEvent(msg)));
        }
    }
}

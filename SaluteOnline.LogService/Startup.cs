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
using SaluteOnline.Domain.Domain.Mongo;
using SaluteOnline.Domain.DTO.Activity;
using SaluteOnline.LogService.DAL;
using SaluteOnline.LogService.Handlers;
using SaluteOnline.LogService.Handlers.Abstraction;

namespace SaluteOnline.LogService
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
            services.AddMvc();
            services.AddSingleton<IGenericRepository<Activity>, GenericRepository<Activity>>();
            services.AddSingleton(typeof(IHandler<>), typeof(Handler<>));
            services.AddRawRabbit(GetRabbitConfiguration);
            InitializeRabbit(services);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
        }

        private static RawRabbitOptions GetRabbitConfiguration => new RawRabbitOptions
        {
            ClientConfiguration = new RawRabbitConfiguration
            {
                Username = "guest",
                Password = "guest",
                VirtualHost = "/",
                Port = 32770,
                Hostnames = new List<string> { "127.0.0.1" },
                Ssl = new SslOption
                {
                    Enabled = false
                },
                AutomaticRecovery = true,
                RecoveryInterval = TimeSpan.FromSeconds(5)
            },
        };

        private static async void InitializeRabbit(IServiceCollection services)
        {
            var bus = RawRabbitFactory.CreateSingleton(GetRabbitConfiguration);
            var handler = services.BuildServiceProvider().GetService<IHandler<Activity>>();
            await bus.SubscribeAsync<ActivitySet>(msg =>
            {                
                handler.HandleAndInsert<Activity>(new Activity
                {
                    Guid = Guid.NewGuid(),
                    Importance = msg.Importance,
                    UserId = msg.UserId,
                    Created = DateTimeOffset.UtcNow,
                    Type = msg.Type,
                    Data = msg.Data
                });
                return Task.FromResult(true);
            });
        }
    }
}

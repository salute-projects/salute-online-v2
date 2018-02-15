using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RawRabbit;
using RawRabbit.Configuration;
using RawRabbit.DependencyInjection.ServiceCollection;
using RawRabbit.Instantiation;
using SaluteOnline.Domain.Events;
using SaluteOnline.HubService.DAL;
using SaluteOnline.HubService.Handlers.Declaration;
using SaluteOnline.HubService.Hubs;
using SaluteOnline.HubService.Security;

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
            app.UseSignalR(routes =>
            {
                routes.MapHub<SoMessageHub>("soMessageHub");
            });
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
                Hostnames = new List<string> { "127.0.0.1" }
            }
        };

        private static async void SubsribeToRabbit(IServiceCollection services)
        {
            var bus = RawRabbitFactory.CreateSingleton(GetRabbitConfiguration);
            var handler = services.BuildServiceProvider().GetService<IHubHandler>();
            await bus.SubscribeAsync<HubEvent>(msg => Task.FromResult(handler.HandleEvent(msg)));
        }
    }
}

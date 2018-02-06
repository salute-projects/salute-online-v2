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
using SaluteOnline.Domain.Events;
using SaluteOnline.MailService.Model;
using SaluteOnline.MailService.Services.Declaration;
using SaluteOnline.MailService.Services.Implementation;

namespace SaluteOnline.MailService
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
            services.AddSingleton<ISesHandler, SesHandler>();
            services.AddRawRabbit(GetRabbitConfiguration);
            services.Configure<AwsSettings>(Configuration.GetSection("AwsSettings"));
            InitializeRabbit(services);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            
        }

        private RawRabbitOptions GetRabbitConfiguration => new RawRabbitOptions
        {
            ClientConfiguration = new RawRabbitConfiguration
            {
                Username = Configuration.GetSection("RabbitSettings").GetValue<string>(nameof(RawRabbitConfiguration.Username)),
                Password = Configuration.GetSection("RabbitSettings").GetValue<string>(nameof(RawRabbitConfiguration.Password)),
                VirtualHost = Configuration.GetSection("RabbitSettings").GetValue<string>(nameof(RawRabbitConfiguration.VirtualHost)),
                Port = Configuration.GetSection("RabbitSettings").GetValue<int>(nameof(RawRabbitConfiguration.Port)),
                Hostnames = Configuration.GetSection("RabbitSettings").GetSection("Hostnames").Get<List<string>>(),
                Ssl = new SslOption
                {
                    Enabled = Configuration.GetSection("RabbitSettings").GetValue<bool>("SslEnabled")
                },
                AutomaticRecovery = Configuration.GetSection("RabbitSettings").GetValue<bool>(nameof(RawRabbitConfiguration.AutomaticRecovery)),
                RecoveryInterval = TimeSpan.FromSeconds(Configuration.GetSection("RabbitSettings").GetValue<int>(nameof(RawRabbitConfiguration.RecoveryInterval)))
            },
        };

        private async void InitializeRabbit(IServiceCollection services)
        {
            var bus = RawRabbitFactory.CreateSingleton(GetRabbitConfiguration);
            var handler = services.BuildServiceProvider().GetService<ISesHandler>();
            await bus.SubscribeAsync<SendEmailEvent>(msg => Task.FromResult(handler.HandleEmail(msg)));
        }
    }
}

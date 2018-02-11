using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace SaluteOnline.Gateway
{
    public class Program
    {
        public static void Main(string[] args)
        {
            IWebHostBuilder builder = new WebHostBuilder();
            builder.ConfigureServices(s =>
            {
                s.AddSingleton(builder);
            });
            builder.UseKestrel()
               .UseContentRoot(Directory.GetCurrentDirectory())
               .ConfigureLogging((context, logging) =>
                {
                    logging.AddConsole();
                })
               .UseStartup<Startup>()
               .UseUrls("http://localhost:9000");
            var host = builder.Build();
            host.Run();
        }
    }
}

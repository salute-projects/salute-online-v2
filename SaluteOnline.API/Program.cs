using System.Diagnostics;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace SaluteOnline.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            if (Debugger.IsAttached)
            {
                Process.Start(@"D:\Projects\startMongo.bat");
            }
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseUrls("http://localhost:9001")
                .Build();
    }
}

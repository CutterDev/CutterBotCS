using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace CutterBotCS.JobService
{
    internal class Program
    {
        static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
        .UseSystemd()
        .ConfigureServices((hostContext, services) =>
        {
            services.AddHostedService<Worker>();
        });
    }
}

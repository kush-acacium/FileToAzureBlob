using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

using System;
using System.IO;
using System.Threading.Tasks;

namespace FileToAzureBlob
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // create service collection
            var services = new ServiceCollection();
            ConfigureServiceCollection(services);

            // create service provider
            var serviceProvider = services.BuildServiceProvider();

            // entry to run app
            await serviceProvider.GetService<App>().Run(args);

            Console.WriteLine("This is a test");
            Console.ReadKey();
            /*var appSettings = serviceProvider.GetService<IOptions<FileToBlobSettings>>().Value;
            Console.WriteLine(appSettings.ConnectionString);
            Console.ReadLine();*/
        }


        private static void ConfigureServiceCollection(IServiceCollection services)
        {
            var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            // build config
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile($"appSettings.json", false, true)
                .AddJsonFile($"appSettings.{environmentName}.json", true, true)
                .AddEnvironmentVariables()
                .Build();

            // setup config
            services.AddOptions();
            services.Configure<FileToBlobSettings>(configuration.GetSection("FileToBlob"));

            // add services:
            // services.AddTransient<IMyRespository, MyConcreteRepository>();

            // add app
            services.AddTransient<App>();
        }
    }
}

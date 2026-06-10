using BC2Gescom.Configuration;
using BC2Gescom.Services;
using FabToolKit.Tracing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;

namespace BC2Gescom
{
    public class Program
    {
        static void Main(string[] args)
        {
            string logFilePath = @$"C:\temp\BC2Gescom\SyncTrace_{DateTime.Now.ToString("dd-MMM-yyyy-HH-mm-ss")}.txt";
            //INSTALL PACKAGES - Microsoft.Extensions.DependencyInjection, Microsoft.Extensions.Configuration.Json

            //create service collection for DI
            ServiceCollection serviceCollection = new ServiceCollection();

            // build a configuration
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetParent(AppContext.BaseDirectory).FullName)
                .AddJsonFile("appsettings.json");

            IConfiguration config = builder.Build();


            //add the config to the service collection
            serviceCollection.AddSingleton<IConfiguration>(config);
            serviceCollection.Configure<ConnectionStrings>(config.GetSection("ConnectionStrings"));
            serviceCollection.Configure<BCTableNames>(config.GetSection("BCTables"));
            serviceCollection.Configure<GescomTableNames>(config.GetSection("GescomTables"));
            serviceCollection.Configure<BCApiInformation>(config.GetSection("BCApiInformation"));

            //add other services
            serviceCollection.AddSingleton<BCDataAccess>();
            serviceCollection.AddSingleton<GescomDataAccess>();
            serviceCollection.AddSingleton<SyncConductor>();

            // build the service provider
            var serviceProvider = serviceCollection.BuildServiceProvider();

            //run app
            var app = serviceProvider.GetService<SyncConductor>();

            Console.WriteLine($"Begin BC 2 Gescom Sync");
            Console.WriteLine($"The log file for this sync can be found here: {logFilePath}");

            Trace.Listeners.Clear();
            Trace.Listeners.Add(new FormattedConsoleTraceListener());
            Trace.Listeners.Add(new FormattedTextWriterTraceListener(logFilePath));

            app!.Run().GetAwaiter().GetResult();

            Trace.Flush();

            Console.WriteLine("Sync complete, press any key to close the window");
            Console.ReadKey();

        }
    }
}

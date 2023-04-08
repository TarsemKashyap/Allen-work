using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Extensions.Logging;
using Infrastructure;

internal class Program
{
    private static async Task Main(string[] args)
    {
        var logger = LogManager.GetCurrentClassLogger();
        try
        {
            string location = System.AppContext.BaseDirectory;
            string directory = Directory.GetCurrentDirectory();
            logger.Info($"location:{location},directory:{directory}");
            IConfiguration config = new ConfigurationBuilder()
               .SetBasePath(location) //From NuGet Package Microsoft.Extensions.Configuration.Json
               .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
               .Build();


            var serviceCollection = new ServiceCollection();

            serviceCollection.AddSingleton<IConfiguration>(config)
            .AddTransient(x => new DbContext(config.GetConnectionString("DbConn")))
            .AddTransient<FileReader>() // Runner is the custom class
            .AddLogging(loggingBuilder =>
            {
                // configure Logging with NLog
                loggingBuilder.ClearProviders();
                loggingBuilder.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
                loggingBuilder.AddNLog(config);
            });

            serviceCollection.Configure<EmailSetting>(config.GetSection("EmailSetting"));

            var servicesProvider = serviceCollection.BuildServiceProvider();

            var runner = servicesProvider.GetRequiredService<FileReader>();
            await runner.Import();
            Console.WriteLine("Scheduler executed successfully");
        }
        catch (Exception ex)
        {
            // NLog: catch any exception and log it.
            logger.Error(ex, ex.Message);
            throw;
        }
        finally
        {
            // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
            LogManager.Shutdown();
        }
    }

}
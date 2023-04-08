using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Extensions.Logging;
using Microsoft.Extensions.Options;


internal class Program
{
    private static async Task Main(string[] args)
    {
        var logger = LogManager.GetCurrentClassLogger();
        try
        {
            IConfiguration config = new ConfigurationBuilder()
              .SetBasePath(System.AppContext.BaseDirectory) //From NuGet Package Microsoft.Extensions.Configuration.Json
              .AddJsonFile("appsettings.json",false,true)
              .Build();

            using var servicesProvider = new ServiceCollection()
                .AddSingleton<IConfiguration>(config)
                .AddTransient(x => new DbContext(config.GetConnectionString("DbConn")))
                .AddTransient<FileReader>() // Runner is the custom class   
                .AddLogging(loggingBuilder =>
                {
                    // configure Logging with NLog
                    loggingBuilder.ClearProviders();
                    loggingBuilder.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
                    loggingBuilder.AddNLog(config);
                })
                .BuildServiceProvider();

            var runner = servicesProvider.GetRequiredService<FileReader>();
            await runner.Import();
            Console.WriteLine("Import successful");
        }
        catch (Exception ex)
        {
            // NLog: catch any exception and log it.
            logger.Error(ex, "Stopped program because of exception");
            throw;
        }
        finally
        {
            // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
            LogManager.Shutdown();
        }
    }

}
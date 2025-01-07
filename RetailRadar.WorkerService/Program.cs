using RetailRadar.App.PageScrappers.Deporvillage;
using RetailRadar.App.Services;

using Serilog;
namespace RetailRadar.WorkerService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Configure Serilog
            Log.Logger = new LoggerConfiguration()
                .WriteTo.File(@"C:\Logs\RetailRadar\log-.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            try
            {
                Log.Information("Starting up the service");

                var builder = Host.CreateDefaultBuilder(args)
                    .UseWindowsService(options =>
                    {
                        options.ServiceName = "Retail Radar";
                    })
                    .UseSerilog() // Add Serilog to the logging system
                    .ConfigureServices((hostContext, services) =>
                    {
                        services.AddHostedService<PriceDropAlertScheduler>();
                        services.AddSingleton<IPriceCheckerService, PriceCheckerService>();
                        services.AddSingleton<IDeporvillageProductPage, DeporvillageProductPage>();
                    });

                var host = builder.Build();
                host.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "There was a problem starting the service");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}
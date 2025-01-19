using Microsoft.Extensions.Configuration;

using Quartz;
using Quartz.AspNetCore;

using RetailRadar.App.Interfaces.Notifications;
using RetailRadar.App.Jobs;
using RetailRadar.App.PageScrappers;
using RetailRadar.App.Services;
using RetailRadar.Notifications.Email;
namespace RetailRadar.QuartzScheduler
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var builder = Host.CreateApplicationBuilder(args);

            RegisterServices(builder);
            InitializeQuartz(builder);

            var host = builder.Build();
            host.Run();
        }
        private static void RegisterServices(HostApplicationBuilder builder)
        {
            // Register your services
            builder.Services.AddSingleton<IPriceCheckerService, PriceCheckerService>();
            builder.Services.AddSingleton<IWebScrapperFactory, WebScrapperFactory>();
            builder.Services.AddSingleton<IPriceCheckerService, PriceCheckerService>();
            builder.Services.AddSingleton<INotificationService, EmailNotificationService>();
        }
        private static void InitializeQuartz(HostApplicationBuilder builder)
        {
            builder.Services.AddQuartz(q =>
            {
                var priceAlerts = builder.Configuration
                    .GetSection("PriceAlerts")
                    .Get<List<PriceAlertConfig>>() ?? new List<PriceAlertConfig>();

                // Configure jobs from settings
                foreach (var alert in priceAlerts)
                {
                    ConfigurePriceCheckJob(q, alert);
                }
            });

            builder.Services.AddQuartzServer(options =>
            {
                options.WaitForJobsToComplete = true;
            });
        }

        private static void ConfigurePriceCheckJob(IServiceCollectionQuartzConfigurator q, PriceAlertConfig alert)
        {
            var jobKey = new JobKey($"PriceCheck_{alert.ProductName}");

            q.AddJob<PriceCheckJob>(opts => opts
                .WithIdentity(jobKey)
                .UsingJobData("ProductUrl", alert.ProductUrl)
                .UsingJobData("PriceThreshold", alert.PriceThreshold.ToString())
                .UsingJobData("RecipientEmail", alert.RecipientEmail)
                .UsingJobData("RecipientName", alert.RecipientName)
                .UsingJobData("ProductScrapperType", alert.ProductScrapperType));

            q.AddTrigger(opts => opts
                .ForJob(jobKey)
                .WithIdentity($"PriceCheckTrigger_{alert.ProductName}")
                .WithCronSchedule(alert.CronExpression));
        }
    }
}
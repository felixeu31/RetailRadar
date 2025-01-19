using System.Globalization;

using Microsoft.Extensions.Logging;
using Quartz;

using RetailRadar.App.Models;
using RetailRadar.App.PageScrappers;
using RetailRadar.App.Services;

namespace RetailRadar.App.Jobs;

public class PriceCheckJob : IJob
{
    private readonly IPriceCheckerService _priceCheckerService;
    private readonly ILogger<PriceCheckJob> _logger;

    public PriceCheckJob(
        IPriceCheckerService priceCheckerService,
        IWebScrapperFactory scrapperFactory,
        ILogger<PriceCheckJob> logger)
    {
        _priceCheckerService = priceCheckerService;
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        var dataMap = context.JobDetail.JobDataMap;
        var productUrl = dataMap.GetString("ProductUrl");
        var priceThreshold = Convert.ToDecimal(dataMap.GetString("PriceThreshold"), CultureInfo.InvariantCulture);
        var scrapperType = dataMap.GetString("ProductScrapperType");
        var recipientName = dataMap.GetString("RecipientName");
        var recipientEmail = dataMap.GetString("RecipientEmail");
        var recipient = new PersonDto(recipientName!, recipientEmail!);

        await _priceCheckerService.ExcutePriceDropAlertProcess(
            new PriceDropAlertProcessRequest(productUrl!, priceThreshold, scrapperType!, recipient));
    }
}

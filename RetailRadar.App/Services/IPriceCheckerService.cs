using RetailRadar.App.Common;

namespace RetailRadar.App.Services
{
    public interface IPriceCheckerService
    {
        Task<Result<Price?>> ExcutePriceDropAlertProcess(string productUrl);
    }
}
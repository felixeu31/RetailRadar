using RetailRadar.App.Common;

namespace RetailRadar.App.Services
{
    public interface IPriceCheckerService
    {
        Task<Result> ExcutePriceDropAlertProcess(PriceDropAlertProcessRequest request);
    }
}
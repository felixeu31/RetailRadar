using RetailRadar.App.Common;
using RetailRadar.App.Models;

namespace RetailRadar.App.Services
{
    public interface IPriceCheckerService
    {
        Task<Result<Price?>> ReadPrice(string productUrl);
    }
}
using RetailRadar.App.Common;
using RetailRadar.App.Models;

namespace RetailRadar.App.Interfaces.Notifications;

public interface INotificationService
{
    Task<Result> NotifyProductPriceDrop(ProductInfoDto productInfo, PersonDto notifyTo);
}

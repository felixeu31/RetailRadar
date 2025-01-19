namespace RetailRadar.QuartzScheduler
{
    public class PriceAlertConfig
    {
        public string ProductName { get; set; } = string.Empty;
        public string ProductUrl { get; set; } = string.Empty;
        public string ProductScrapperType { get; set; } = string.Empty;
        public string CronExpression { get; set; } = string.Empty;
        public decimal PriceThreshold { get; set; } = default;
        public string RecipientEmail { get; set; } = string.Empty;
        public string RecipientName { get; set; } = string.Empty;
    }
}
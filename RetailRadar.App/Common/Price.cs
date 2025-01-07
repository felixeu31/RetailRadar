using System.Globalization;

namespace RetailRadar.App.Common
{
    public class Price
    {
        public decimal Amount { get; set; }
        public string Currency { get; set; }

        public static Result<Price> ConvertFromText(string priceText)
        {
            if (string.IsNullOrWhiteSpace(priceText))
            {
                return Result<Price>.Failure("Price text is empty or null");
            }

            // Extract currency symbol
            var currencySymbols = new[] { "€", "$", "£", "¥", "₹" };
            var currency = currencySymbols.FirstOrDefault(symbol => priceText.Contains(symbol)) ?? string.Empty;

            // Remove currency symbol and clean the price text
            var cleanedPriceText = priceText.Trim().Replace(currency, "").Replace(",", ".");

            if (!decimal.TryParse(cleanedPriceText, NumberStyles.Number, CultureInfo.InvariantCulture, out var priceAmount))
            {
                return Result<Price>.Failure("Invalid price format");
            }

            var price = new Price { Amount = priceAmount, Currency = currency };
            return Result<Price>.Success(price);
        }

        public override string ToString()
        {
            return $"{Amount.ToString("F2", CultureInfo.InvariantCulture)} {Currency}";
        }
    }
}

using System.Net;

using RetailRadar.App.Common;
using RetailRadar.App.Interfaces.Notifications;
using RetailRadar.App.Models;

using SendGrid;
using SendGrid.Helpers.Mail;

namespace RetailRadar.Notifications.Email
{
    public class EmailNotificationService : INotificationService
    {
        private readonly SendGridClient _client;
        private readonly EmailAddress _from;
        public EmailNotificationService()
        {
            var apiKey = Environment.GetEnvironmentVariable("PRICE_CHECKER_SENDGRID_KEY");
            _client = new SendGridClient(apiKey);
            _from = new EmailAddress("tunstall.felixeu31@gmail.com", "Felix Sendgrid");
        }

        public async Task<Result> NotifyProductPriceDrop(ProductInfoDto productInfo, PersonDto notifyTo)
        {
            try
            {
                var subject = $"El precio ha bajado para: {productInfo.Name}";
                var to = new EmailAddress(notifyTo.Email, notifyTo.Name);
                var plainTextContent = $"El nuevo precio es de {productInfo.Price.Amount}";
                string htmlContent = CreateEmailBody(productInfo);

                var msg = MailHelper.CreateSingleEmail(_from, to, subject, plainTextContent, htmlContent);

                // Act
                var response = await _client.SendEmailAsync(msg);

                if (response.StatusCode != HttpStatusCode.Accepted)
                {
                    return Result.Failure("Some error occurred while notifying via email");
                }

                return Result.Success();
            }
            catch (Exception ex)
            {

                return Result.Failure($"Exception ocurred while notifying via email: {ex.Message}");
            }
        }

        private static string CreateEmailBody(ProductInfoDto productInfo)
        {
            return @$"
                <html>
                <head>
                    <style>
                        .container {{
                            font-family: Arial, sans-serif;
                            margin: 20px;
                            padding: 20px;
                            border: 1px solid #ddd;
                            border-radius: 10px;
                            background-color: #f9f9f9;
                        }}
                        .header {{
                            font-size: 24px;
                            font-weight: bold;
                            margin-bottom: 20px;
                        }}
                        .content {{
                            font-size: 16px;
                            margin-bottom: 20px;
                        }}
                        .footer {{
                            font-size: 14px;
                            color: #888;
                        }}
                        .button {{
                            display: inline-block;
                            padding: 10px 20px;
                            font-size: 16px;
                            color: #fff;
                            background-color: #007bff;
                            text-decoration: none;
                            border-radius: 5px;
                        }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <div class='header'>¡El precio ha bajado!</div>
                        <div class='content'>
                            <p>El nuevo precio para <strong>{productInfo.Name}</strong> es de <strong>{productInfo.Price.ToString()}€</strong>.</p>
                            <p>Visita la página para comprarlas:</p>
                            <p><a href='{productInfo.Url}' class='button'>Comprar ahora</a></p>
                        </div>
                        <div class='footer'>
                            <p>Gracias por usar nuestro servicio de alertas de precios.</p>
                        </div>
                    </div>
                </body>
                </html>";
        }
    }
}

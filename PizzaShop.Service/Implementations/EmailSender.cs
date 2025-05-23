using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using Microsoft.Extensions.Configuration;
using PizzaShop.Service.Interfaces;
using Microsoft.AspNetCore.Hosting;

namespace PizzaShop.Service.Implementations
{
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public EmailSender(IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            _configuration = configuration;
            _webHostEnvironment = webHostEnvironment;
        }
        public async Task SendEmailAsync(string toEmail, string subject, string htmlBody, string imagePath)
        {
            var smtpHost = _configuration["EmailSettings:SmtpHost"];
            var smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"]!);
            var smtpUser = _configuration["EmailSettings:SmtpUser"];
            var smtpPass = _configuration["EmailSettings:SmtpPass"];
            var fromEmail = _configuration["EmailSettings:FromEmail"];

            using (var client = new SmtpClient(smtpHost, smtpPort))
            {
                client.Credentials = new NetworkCredential(smtpUser, smtpPass);
                client.EnableSsl = true;

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(fromEmail ?? string.Empty, "PizzaShop Support"),
                    Subject = subject,
                    IsBodyHtml = true
                };

                mailMessage.To.Add(toEmail);

                var alternateView = AlternateView.CreateAlternateViewFromString(htmlBody, null, MediaTypeNames.Text.Html);

                if (File.Exists(imagePath))
                {
                    LinkedResource logo = new LinkedResource(imagePath, MediaTypeNames.Image.Jpeg)
                    {
                        ContentId = "PizzaShopLogo",
                        TransferEncoding = TransferEncoding.Base64,
                        ContentType = new ContentType("image/png")
                    };

                    alternateView.LinkedResources.Add(logo);
                }

                mailMessage.AlternateViews.Add(alternateView);

                await client.SendMailAsync(mailMessage);
            }
        }

        public async Task SendResetPasswordEmail(string toEmail, string? resetLink)
        {
            var imagePath = Path.Combine(_webHostEnvironment.WebRootPath, "images", "logo.png");

            string subject = "Reset Your Password - PizzaShop";

            string body = $@"
<html>
<body style='font-family: Arial, sans-serif; margin: 0; padding: 0;'>
    <table role='presentation' width='100%' cellspacing='0' cellpadding='0' border='0' style='background-color: #f5f5f5; padding: 20px 0; text-align: center;'>
        <tr>
            <td align='center'>
                <table role='presentation' width='600px' cellspacing='0' cellpadding='0' border='0' style='background: white; padding: 20px; text-align: center;'>
                    <tr>
                        <td style='background-color: #0066A7; padding: 20px; text-align: center;'>
                            <table role='presentation' cellspacing='0' cellpadding='0' border='0' align='center'>
                                <tr>
                                    <td valign='middle'>
                                        <!-- Ensure image size is 20px by 20px using inline CSS -->
                                        <div style='display: flex; align-items: center; justify-content: center;'>
                                            <!-- Image -->
                                            <img src='cid:PizzaShopLogo' alt='PizzaShop Logo' style='width: 40px; height: 40px; border-radius: 5px; margin-right: 10px;' />
                                            <!-- Text (h2 with inline-block) -->
                                            <h2 style='color: white; margin: 0; font-size: 22px; display: inline-block;'>PIZZASHOP</h2>
                                        </div>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td style='text-align: left; padding: 20px; font-size: 16px; color: #333;'>
                            <p>Dear Pizza Shop User,</p>
                            <p>Please <a href='{resetLink}' style='color: blue;'>click here</a> to reset your account password.</p>
                            <p>If you encounter any issues or have any questions, please do not hesitate to contact our support team.</p>
                            <p style='color: red; font-weight: bold;'>Important Note: For security reasons, the link will expire in 24 hours.</p>
                            <p>If you did not request a password reset, please ignore this email or contact our support team immediately.</p>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</body>
</html>";

            await SendEmailAsync(toEmail, subject, body, imagePath);
        }

    }
}

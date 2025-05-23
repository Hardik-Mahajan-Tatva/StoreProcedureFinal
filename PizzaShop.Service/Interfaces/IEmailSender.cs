namespace PizzaShop.Service.Interfaces
{
    public interface IEmailSender
    {
        /// <summary>
        /// Sends an email asynchronously with the specified details.
        /// </summary>
        /// <param name="toEmail">The recipient's email address.</param>
        /// <param name="subject">The subject of the email.</param>
        /// <param name="body">The body content of the email.</param>
        /// <param name="imagePath">The path to an image to include in the email (optional).</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task SendEmailAsync(string toEmail, string subject, string body, string imagePath);

        /// <summary>
        /// Sends a reset password email asynchronously with a reset link.
        /// </summary>
        /// <param name="toEmail">The recipient's email address.</param>
        /// <param name="resetLink">The password reset link to include in the email.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task SendResetPasswordEmail(string toEmail, string? resetLink);
    }
}

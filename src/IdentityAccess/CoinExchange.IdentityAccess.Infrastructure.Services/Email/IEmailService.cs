using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace CoinExchange.IdentityAccess.Infrastructure.Services.Email
{
    /// <summary>
    /// Interface for the Email Service using SMTP
    /// </summary>
    public interface IEmailService
    {
        /// <summary>
        /// Sends Email to the given recipient
        /// </summary>
        /// <param name="to"></param>
        /// <param name="subject"> </param>
        /// <param name="content"> </param>
        /// <returns></returns>
        bool SendMail(string to, string subject, string content);

        /// <summary>
        /// Send activation key after signup
        /// </summary>
        /// <param name="to"></param>
        /// <param name="activationKey"></param>
        /// <returns></returns>
        bool SendPostSignUpEmail(string to, string activationKey);

        /// <summary>
        /// Sends the email that the user should get when they request us to remind them of their username
        /// </summary>
        /// <param name="to"></param>
        /// <param name="username"></param>
        /// <returns></returns>
        bool SendForgotUsernameEmail(string to, string username);

        /// <summary>
        /// Sends the email that the user should get when they request to reset their password
        /// </summary>
        /// <param name="to"></param>
        /// <param name="passwordResetLink"></param>
        /// <returns></returns>
        bool SendForgotPasswordEmail(string to, string passwordResetLink);

        /// <summary>
        /// Sends Welcome Email
        /// </summary>
        /// <param name="to"></param>
        /// <param name="username"></param>
        /// <returns></returns>
        bool SendWelcomeEmail(string to, string username);

        /// <summary>
        /// Email the user that they just changed password
        /// </summary>
        /// <param name="to"></param>
        /// <param name="username"></param>
        /// <returns></returns>
        bool SendPasswordChangedEmail(string to, string username);

        /// <summary>
        /// Instance of the SMTP Client
        /// </summary>
        SmtpClient SmtpClient { get; }

        /// <summary>
        /// The address from which the Email will be sent
        /// </summary>
        string FromAddress { get; }
    }
}

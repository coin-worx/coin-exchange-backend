using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.IdentityAccess.Infrastructure.Services.Email;

namespace CoinExchange.IdentityAccess.Application.Tests
{
    /// <summary>
    /// Mocking Email Service
    /// </summary>
    public class MockEmailService : IEmailService
    {
        /// <summary>
        /// Send Email
        /// </summary>
        /// <param name="to"></param>
        /// <param name="subject"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public bool SendMail(string to, string subject, string content)
        {
            return true;
        }

        /// <summary>
        /// For post sign up email
        /// </summary>
        /// <param name="to"></param>
        /// <param name="activationKey"></param>
        /// <returns></returns>
        public bool SendPostSignUpEmail(string to, string activationKey)
        {
            return true;
        }

        /// <summary>
        /// For forgot username
        /// </summary>
        /// <param name="to"></param>
        /// <param name="username"></param>
        /// <returns></returns>
        public bool SendForgotUsernameEmail(string to, string username)
        {
            return true;
        }

        /// <summary>
        /// Sends the email that the user should get when they request to reset their password
        /// </summary>
        /// <param name="to"></param>
        /// <param name="passwordResetLink"></param>
        /// <returns></returns>
        public bool SendForgotPasswordEmail(string to, string passwordResetLink)
        {
            return true;
        }

        /// <summary>
        /// SMTP Client
        /// </summary>
        public SmtpClient SmtpClient { get; private set; }

        /// <summary>
        /// From Address
        /// </summary>
        public string FromAddress { get; private set; }
    }
}

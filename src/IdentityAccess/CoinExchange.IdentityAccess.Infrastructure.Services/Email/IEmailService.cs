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
        /// Instance of the SMTP Client
        /// </summary>
        SmtpClient SmtpClient { get; }

        /// <summary>
        /// The address from which the Email will be sent
        /// </summary>
        string FromAddress { get; }
    }
}

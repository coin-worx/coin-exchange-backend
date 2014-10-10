using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace CoinExchange.IdentityAccess.Infrastructure.Services.Email
{
    /// <summary>
    /// Stub Email Service
    /// </summary>
    public class StubEmailService : IEmailService
    {
        public bool SendMail(string to, string subject, string content, bool adminEmailsAllowed)
        {
            return true;
        }

        public bool SendPostSignUpEmail(string to, string username, string activationKey, bool adminEmailsAllowed)
        {
            return true;
        }

        public bool SendForgotUsernameEmail(string to, string username, bool adminEmailsAllowed)
        {
            return true;
        }

        public bool SendWelcomeEmail(string to, string username, bool adminEmailsAllowed)
        {
            return true;
        }

        public bool SendPasswordChangedEmail(string to, string username, bool adminEmailsAllowed)
        {
            return true;
        }

        public bool SendReactivaitonNotificationEmail(string to, string username, bool adminEmailsAllowed)
        {
            return true;
        }

        public bool SendCancelActivationEmail(string to, string username, bool adminEmailsAllowed)
        {
            return true;
        }

        public SmtpClient SmtpClient { get; private set; }
        public string FromAddress { get; private set; }
    }
}

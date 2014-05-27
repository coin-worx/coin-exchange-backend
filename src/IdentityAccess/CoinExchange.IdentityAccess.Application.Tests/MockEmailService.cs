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
        public bool SendMail(string to, string subject, string content)
        {
            return true;
        }

        public SmtpClient SmtpClient { get; private set; }
        public string FromAddress { get; private set; }
    }
}

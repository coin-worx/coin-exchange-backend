using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinExchange.IdentityAccess.Application.UserServices.Commands
{
    /// <summary>
    /// Command for submitting the email Settings for a user
    /// </summary>
    public class EmailSettingsCommand
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public EmailSettingsCommand(string apiKey, bool adminEmailsSubscribed, bool newsLetterEmailsSubscribed)
        {
            ApiKey = apiKey;
            AdminEmailsSubscribed = adminEmailsSubscribed;
            NewsLetterEmailsSubscribed = newsLetterEmailsSubscribed;
        }

        /// <summary>
        /// API Key
        /// </summary>
        public string ApiKey { get; private set; }

        /// <summary>
        /// Admin Emails
        /// </summary>
        public bool AdminEmailsSubscribed { get; private set; }

        /// <summary>
        /// Newsletter emails
        /// </summary>
        public bool NewsLetterEmailsSubscribed { get; private set; }
    }
}

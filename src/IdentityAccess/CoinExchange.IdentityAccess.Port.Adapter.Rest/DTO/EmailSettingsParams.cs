using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinExchange.IdentityAccess.Port.Adapter.Rest.DTO
{
    /// <summary>
    /// Parametrs for email settings and notifications
    /// </summary>
    public class EmailSettingsParams
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public EmailSettingsParams(bool administrativeEmails, bool newsLetterEmails)
        {
            AdministrativeEmails = administrativeEmails;
            NewsLetterEmails = newsLetterEmails;
        }

        /// <summary>
        /// Admin emails
        /// </summary>
        public bool AdministrativeEmails { get; private set; }

        /// <summary>
        /// Newsletter emails
        /// </summary>
        public bool NewsLetterEmails { get; private set; }
    }
}

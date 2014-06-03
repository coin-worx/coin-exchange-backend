using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.IdentityAccess.Domain.Model.UserAggregate;

namespace CoinExchange.IdentityAccess.Port.Adapter.Rest.DTO
{
    /// <summary>
    /// Parameters for changing the user's account settings
    /// </summary>
    public class ChangeSettingsParams
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public ChangeSettingsParams(string email, string pgpPublicKey, Language language, TimeZone timeZone, bool isDefaultAutoLogout, int autoLogoutMinutes)
        {
            Email = email;
            PgpPublicKey = pgpPublicKey;
            Language = language;
            TimeZone = timeZone;
            IsDefaultAutoLogout = isDefaultAutoLogout;
            AutoLogoutMinutes = autoLogoutMinutes;
        }
        
        /// <summary>
        /// Email
        /// </summary>
        public string Email { get; private set; }

        /// <summary>
        /// PGP Public Key
        /// </summary>
        public string PgpPublicKey { get; private set; }

        /// <summary>
        /// Language
        /// </summary>
        public Language Language { get; private set; }

        /// <summary>
        /// TimeZone
        /// </summary>
        public TimeZone TimeZone { get; private set; }

        /// <summary>
        /// Specifies if the Auto logout time is the Custom(specified by the user betwenn 2 and 240 minutes) or Default
        /// </summary>
        public bool IsDefaultAutoLogout { get; private set; }

        /// <summary>
        /// The minutes after which the User will logout automatically
        /// </summary>
        public int AutoLogoutMinutes { get; private set; }
    }
}

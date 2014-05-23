using System;

namespace CoinExchange.IdentityAccess.Application.RegistrationServices.Commands
{
    /// <summary>
    /// Command to register a user for CoinExchange services
    /// </summary>
    public class SignupUserCommand
    {
        /// <summary>
        /// Parameterized constructor
        /// </summary>
        /// <param name="email"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="country"></param>
        /// <param name="timeZone"></param>
        /// <param name="pgpPublicKey"></param>
        public SignupUserCommand(string email, string username, string password, string country, TimeZone timeZone, string pgpPublicKey)
        {
            Email = email;
            Username = username;
            Password = password;
            Country = country;
            TimeZone = timeZone;
            PgpPublicKey = pgpPublicKey;
        }

        /// <summary>
        /// Email
        /// </summary>       
        public string Email { get; private set; }

        /// <summary>
        /// Username
        /// </summary>       
        public string Username { get; private set; }

        /// <summary>
        /// Password
        /// </summary>       
        public string Password { get; private set; }

        /// <summary>
        /// Country
        /// </summary>  
        public string Country { get; private set; }

        /// <summary>
        /// TimeZone
        /// </summary>       
        public TimeZone TimeZone { get; private set; }

        /// <summary>
        /// PGPPublicKey
        /// </summary>       
        public string PgpPublicKey { get; private set; }
    }
}

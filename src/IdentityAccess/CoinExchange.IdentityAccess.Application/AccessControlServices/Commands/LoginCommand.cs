namespace CoinExchange.IdentityAccess.Application.AccessControlServices.Commands
{
    /// <summary>
    /// Commandspassed to application service for login of a user
    /// </summary>
    public class LoginCommand
    {
        /// <summary>
        /// Accepts username and password
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        public LoginCommand(string username, string password)
        {
            Username = username;
            Password = password;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public LoginCommand(string username, string password, string mfaCode)
        {
            Username = username;
            Password = password;
            MfaCode = mfaCode;
        }

        /// <summary>
        /// Username
        /// </summary>
        public string Username { get; private set; }

        /// <summary>
        /// Password
        /// </summary>
        public string Password { get; private set; }

        /// <summary>
        /// Mfa Code
        /// </summary>
        public string MfaCode { get; private set; }
    }
}

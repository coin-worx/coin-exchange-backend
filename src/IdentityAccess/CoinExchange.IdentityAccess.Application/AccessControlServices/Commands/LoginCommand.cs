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
        /// Username
        /// </summary>
        public string Username { get; private set; }

        /// <summary>
        /// Password
        /// </summary>
        public string Password { get; private set; }
    }
}

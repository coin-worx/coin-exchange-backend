using CoinExchange.IdentityAccess.Domain.Model.SecurityKeysAggregate;

namespace CoinExchange.IdentityAccess.Application.AccessControlServices.Commands
{
    /// <summary>
    /// Command to request the logout of a user
    /// </summary>
    public class LogoutCommand
    {
        /// <summary>
        /// Parameterized Constructor
        /// </summary>
        /// <param name="validationEssentials"></param>
        public LogoutCommand(UserValidationEssentials validationEssentials)
        {
            ValidationEssentials = validationEssentials;
        }

        /// <summary>
        /// Contains the API and Secret Key given to user after Logging In
        /// </summary>
        public UserValidationEssentials ValidationEssentials { get; private set; }
    }
}

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
        /// <param name="apiKey"> </param>
        /// <param name="secretKey"> </param>
        public LogoutCommand(string apiKey)
        {
            ApiKey = new ApiKey(apiKey);
            //SecretKey = new SecretKey(secretKey);
        }

        /// <summary>
        /// API Key
        /// </summary>
        public ApiKey ApiKey { get; private set; }

        /// <summary>
        /// Secret Key
        /// </summary>
       // public SecretKey SecretKey { get; private set; }
    }
}

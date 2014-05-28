using System.Security.Authentication;
using CoinExchange.IdentityAccess.Application.AccessControlServices.Commands;
using CoinExchange.IdentityAccess.Domain.Model.Repositories;
using CoinExchange.IdentityAccess.Domain.Model.SecurityKeysAggregate;

namespace CoinExchange.IdentityAccess.Application.AccessControlServices
{
    /// <summary>
    /// Service to serve the loging out process for a user
    /// </summary>
    public class LogoutApplicationService : ILogoutApplicationService
    {
        private ISecurityKeysRepository _securityKeysRepository;
        /// <summary>
        /// Parameterized Constructor
        /// </summary>
        /// <param name="securityKeysRepository"></param>
        public LogoutApplicationService(ISecurityKeysRepository securityKeysRepository)
        {
            _securityKeysRepository = securityKeysRepository;
        }

        /// <summary>
        /// Logs the user out
        /// </summary>
        /// <returns></returns>
        public bool Logout(LogoutCommand logoutCommand)
        {
            if (logoutCommand.ValidationEssentials != null && 
                logoutCommand.ValidationEssentials.ApiKey != null && 
                !string.IsNullOrEmpty(logoutCommand.ValidationEssentials.ApiKey.Value) &&
                logoutCommand.ValidationEssentials.SecretKey != null &&
                !string.IsNullOrEmpty(logoutCommand.ValidationEssentials.SecretKey.Value))
            {
                SecurityKeysPair securityKeysPair =
                    _securityKeysRepository.GetByApiKey(logoutCommand.ValidationEssentials.ApiKey.Value);

                // ToDo: Test after Bilal Finishes Implementation ofthis deletion
                _securityKeysRepository.DeleteSecurityKeysPair(securityKeysPair);
                return false;
            }
            else
            {
                throw new InvalidCredentialException("Invalid or Incomplete Logout Credentials");
            }
        }
    }
}

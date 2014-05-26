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
            SecurityKeysPair securityKeysPair = _securityKeysRepository.GetByApiKey(logoutCommand.ValidationEssentials.SecurityKeys.ApiKey.Value);

            // ToDO: Soft Delete the digital signature after the feature is implemented by Bilal in the repository
            return false;
        }
    }
}

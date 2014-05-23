using CoinExchange.IdentityAccess.Application.AccessControlServices.Commands;
using CoinExchange.IdentityAccess.Domain.Model.SecurityKeysAggregate;

namespace CoinExchange.IdentityAccess.Application.AccessControlServices
{
    /// <summary>
    /// Interface for operations relted to Login
    /// </summary>
    public interface ILoginApplicationService
    {
        /// <summary>
        /// Requests the login for a user
        /// </summary>
        /// <param name="loginCommand"> </param>
        /// <returns></returns>
        UserValidationEssentials Login(LoginCommand loginCommand);
    }
}

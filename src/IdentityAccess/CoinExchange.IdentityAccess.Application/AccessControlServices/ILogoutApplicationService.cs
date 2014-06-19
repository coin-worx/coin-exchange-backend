using CoinExchange.IdentityAccess.Application.AccessControlServices.Commands;

namespace CoinExchange.IdentityAccess.Application.AccessControlServices
{
    /// <summary>
    /// Interface for logging out
    /// </summary>
    public interface ILogoutApplicationService
    {
        /// <summary>
        /// Resuests to log the user out
        /// </summary>
        /// <returns></returns>
        bool Logout(LogoutCommand logoutCommand);
    }
}

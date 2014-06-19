using CoinExchange.IdentityAccess.Application.RegistrationServices.Commands;

namespace CoinExchange.IdentityAccess.Application.RegistrationServices
{
    /// <summary>
    /// IRegistrationApplicationService
    /// </summary>
    public interface IRegistrationApplicationService
    {
        /// <summary>
        /// Request from the client to create a new account
        /// </summary>
        /// <param name="signupUserCommand"> </param>
        /// <returns></returns>
        string CreateAccount(SignupUserCommand signupUserCommand);
    }
}

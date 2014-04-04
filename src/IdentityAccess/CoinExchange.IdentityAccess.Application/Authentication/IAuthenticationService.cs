using CoinExchange.IdentityAccess.Application.Authentication.Commands;

namespace CoinExchange.IdentityAccess.Application.Authentication
{
    public interface IAuthenticationService
    {
        bool Authenticate(AuthenticateCommand command);
        string GenerateNonce();
    }
}

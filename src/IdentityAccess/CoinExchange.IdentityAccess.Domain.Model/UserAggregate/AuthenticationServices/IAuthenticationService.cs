using CoinExchange.IdentityAccess.Domain.Model.UserAggregate.AuthenticationServices.Commands;

namespace CoinExchange.IdentityAccess.Domain.Model.UserAggregate.AuthenticationServices
{
    public interface IAuthenticationService
    {
        bool Authenticate(AuthenticateCommand command);
        string GenerateNonce();
    }
}

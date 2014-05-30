namespace CoinExchange.IdentityAccess.Domain.Model.SecurityKeysAggregate
{
    /// <summary>
    /// Digital signature info repository
    /// </summary>
    public interface ISecurityKeysRepository
    {
        SecurityKeysPair GetByKeyDescriptionAndUserId(string keyDescription,int userId);
        SecurityKeysPair GetByApiKey(string apiKey);
        bool DeleteSecurityKeysPair(SecurityKeysPair securityKeysPair);
        SecurityKeysPair GetByDescriptionAndApiKey(string description, string apiKey);
    }
}

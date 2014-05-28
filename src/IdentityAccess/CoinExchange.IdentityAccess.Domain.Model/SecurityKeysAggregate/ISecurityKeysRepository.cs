namespace CoinExchange.IdentityAccess.Domain.Model.SecurityKeysAggregate
{
    /// <summary>
    /// Digital signature info repository
    /// </summary>
    public interface ISecurityKeysRepository
    {
        SecurityKeysPair GetByKeyDescription(string keyDescription,string userName);
        SecurityKeysPair GetByApiKey(string apiKey);
        bool DeleteSecurityKeysPair(SecurityKeysPair securityKeysPair);
    }
}

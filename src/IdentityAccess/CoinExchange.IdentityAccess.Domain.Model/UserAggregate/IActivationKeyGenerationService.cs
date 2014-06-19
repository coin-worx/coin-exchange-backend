namespace CoinExchange.IdentityAccess.Domain.Model.UserAggregate
{
    /// <summary>
    /// Generates a new and unique Activation key
    /// </summary>
    public interface IActivationKeyGenerationService
    {
        string GenerateNewActivationKey();
    }
}

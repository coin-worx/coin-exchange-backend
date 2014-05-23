using CoinExchange.IdentityAccess.Domain.Model.SecurityKeysAggregate;

namespace CoinExchange.IdentityAccess.Application.DigitalSignatureServices
{
    /// <summary>
    /// Interface for Digital Signature Application Service
    /// </summary>
    public interface IDigitalSignatureApplicationService
    {
        /// <summary>
        /// Generates new key
        /// </summary>
        /// <returns></returns>
        DigitalSignature GetNewKey();

        /// <summary>
        /// Set new permission
        /// </summary>
        void SetPermission();

        /// <summary>
        /// Is the API key valid
        /// </summary>
        /// <param name="apiKey"></param>
        /// <returns></returns>
        bool ApiKeyValidation(string apiKey);
    }
}

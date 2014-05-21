using System;

namespace CoinExchange.IdentityAccess.Domain.Model.SecurityKeysAggregate
{
    /// <summary>
    /// API Key
    /// </summary>
    public class SecurityKeysPair
    {
        #region Private Fields

        private ApiKey _apiKey;
        private SecretKey _secretKey;

        #endregion Private Fields

        #region Methods

        /// <summary>
        /// Change the value of the API Key
        /// </summary>
        /// <param name="apiKey"></param>
        /// <returns></returns>
        public bool ChangeApiKeyValue(ApiKey apiKey)
        {
            if (IsApiKeyValid(apiKey))
            {
                _apiKey = apiKey;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Change the value of the Secret Key
        /// </summary>
        /// <param name="secretKey"></param>
        /// <returns></returns>
        public bool ChangeSecretKeyValue(SecretKey secretKey)
        {
            if (IsSecretKeyValid(secretKey))
            {
                _secretKey = secretKey;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Verifies if the given API Key is valid or not
        /// </summary>
        /// <param name="apiKey"></param>
        /// <returns></returns>
        private bool IsApiKeyValid(ApiKey apiKey)
        {
            if (apiKey != null && !string.IsNullOrEmpty(apiKey.Value))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Verifies if the given Secret Key is valid or not
        /// </summary>
        /// <param name="secretKey"></param>
        /// <returns></returns>
        private bool IsSecretKeyValid(SecretKey secretKey)
        {
            if (secretKey != null && !string.IsNullOrEmpty(secretKey.Value))
            {
                return true;
            }
            return false;
        }

        #endregion Methods

        /// <summary>
        /// API Key
        /// </summary>
        public ApiKey ApiKey { get { return _apiKey; } private set { _apiKey = value; } }

        public SecretKey SecretKey { get { return _secretKey; } set { _secretKey = value; } }

        /// <summary>
        /// Expiration Date
        /// </summary>
        public DateTime ExpirationDate { get; private set; }

        /// <summary>
        /// StartDate
        /// </summary>
        public DateTime StartDate { get; private set; }
        
        /// <summary>
        /// EndDate
        /// </summary>
        public DateTime EndDate { get; private set; }

        /// <summary>
        /// LastModified
        /// </summary>
        public DateTime LastModified { get; set; }

        /// <summary>
        /// SystemGenerated
        /// </summary>
        public bool SystemGenerated { get; private set; }
    }
}

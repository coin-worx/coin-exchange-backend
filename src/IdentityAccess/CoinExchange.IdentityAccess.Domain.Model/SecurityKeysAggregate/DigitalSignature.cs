using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinExchange.IdentityAccess.Domain.Model.SecurityKeysAggregate
{
    /// <summary>
    /// Contains the API Key and Secret Key
    /// </summary>
    public class DigitalSignature
    {
        //default constructor
        public DigitalSignature()
        {
            
        }
        /// <summary>
        /// Parameterized Constructor
        /// </summary>
        /// <param name="apiKey"></param>
        /// <param name="secretKey"> </param>
        public DigitalSignature(string apiKey, string secretKey)
        {
            this.ApiKey = apiKey;
            this.SecretKey = secretKey;
        }

        /// <summary>
        /// Value of the API Key
        /// </summary>
        public string ApiKey { get; private set; }

        /// <summary>
        /// Value of the Secret Key
        /// </summary>
        public string SecretKey { get; private set; }

        public override bool Equals(object obj)
        {
            if (obj is DigitalSignature)
            {
                DigitalSignature keys = obj as DigitalSignature;
                return ApiKey.Equals(keys.ApiKey, StringComparison.InvariantCultureIgnoreCase) &&
                       SecretKey.Equals(keys.SecretKey, StringComparison.InvariantCultureIgnoreCase);
            }
            return false;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinExchange.IdentityAccess.Domain.Model.SecurityKeysAggregate
{
    /// <summary>
    /// Secret Key
    /// </summary>
    public class SecretKey
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public SecretKey()
        {
            
        }

        /// <summary>
        /// ParameterizedCOnstrcutor
        /// </summary>
        /// <param name="value"></param>
        public SecretKey(string value)
        {
            Value = value;
        }

        /// <summary>
        /// Value of the API Key
        /// </summary>
        public string Value { get; private set; }
    }
}

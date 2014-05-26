using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinExchange.IdentityAccess.Domain.Model.SecurityKeysAggregate
{
    /// <summary>
    /// Api Key Value Object
    /// </summary>
    public class ApiKey
    {
        /// <summary>
        /// default constructor
        /// </summary>
        public ApiKey()
        {
            
        }

        /// <summary>
        /// ParameterizedCOnstrcutor
        /// </summary>
        /// <param name="value"></param>
        public ApiKey(string value)
        {
            Value = value;
        }

        /// <summary>
        /// Value of the API Key
        /// </summary>
        public string Value { get; private set; }
    }
}

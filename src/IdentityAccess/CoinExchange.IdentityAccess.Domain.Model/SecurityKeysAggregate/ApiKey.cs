using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinExchange.IdentityAccess.Domain.Model.SecurityKeysAggregate
{
    /// <summary>
    /// API Key
    /// </summary>
    public class ApiKey
    {
        /// <summary>
        /// Parameterized Constructor
        /// </summary>
        /// <param name="value"></param>
        public ApiKey(string value)
        {
            this.Value = value;
        }

        /// <summary>
        /// Value of the API Key
        /// </summary>
        public string Value { get; private set; }
    }
}

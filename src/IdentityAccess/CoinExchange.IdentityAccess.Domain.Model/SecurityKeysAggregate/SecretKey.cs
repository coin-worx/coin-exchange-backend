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
        /// Parameterized Constructor
        /// </summary>
        /// <param name="value"></param>
        public SecretKey(string value)
        {
            this.Value = value;
        }

        /// <summary>
        /// Value of the Secret Key
        /// </summary>
        public string Value { get; private set; }
    }
}

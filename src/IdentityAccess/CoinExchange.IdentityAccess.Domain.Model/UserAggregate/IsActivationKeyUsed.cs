using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinExchange.IdentityAccess.Domain.Model.UserAggregate
{
    /// <summary>
    /// Specifies if the Activation Key assiciated with the User has been used
    /// </summary>
    public class IsActivationKeyUsed
    {
        /// <summary>
        /// Default Constructor
        /// </summary>
        public IsActivationKeyUsed()
        {
            
        }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="value"></param>
        public IsActivationKeyUsed(bool value)
        {
            Value = value;
        }

        /// <summary>
        /// Value
        /// </summary>
        public bool Value { get; private set; }
    }
}

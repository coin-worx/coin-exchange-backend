using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinExchange.IdentityAccess.Domain.Model.UserAggregate
{
    /// <summary>
    /// Specifies if the User has been blocked
    /// </summary>
    public class IsUserBlocked
    {
        /// <summary>
        /// default constructor
        /// </summary>
        public IsUserBlocked()
        {
            
        }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="value"></param>
        public IsUserBlocked(bool value)
        {
            Value = value;
        }

        /// <summary>
        /// Value
        /// </summary>
        public bool Value { get; private set; }
    }
}

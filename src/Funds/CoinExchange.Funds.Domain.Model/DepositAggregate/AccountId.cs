using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinExchange.Funds.Domain.Model.DepositAggregate
{
    /// <summary>
    /// AccountID
    /// </summary>
    public class AccountId
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public AccountId()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public AccountId(string value)
        {
            Value = value;
        }

        /// <summary>
        /// Value of the AccountID
        /// </summary>
        public string Value { get; set; }
    }
}

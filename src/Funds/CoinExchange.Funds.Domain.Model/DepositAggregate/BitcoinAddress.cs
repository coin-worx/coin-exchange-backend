using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinExchange.Funds.Domain.Model.DepositAggregate
{
    /// <summary>
    /// Bitcoin Address to make Deposits to or Withdrawals from
    /// </summary>
    public class BitcoinAddress
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public BitcoinAddress()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public BitcoinAddress(string value)
        {
            Value = value;
        }

        /// <summary>
        /// Value of the Bitcoin Address
        /// </summary>
        public string Value { get; private set; }
    }
}

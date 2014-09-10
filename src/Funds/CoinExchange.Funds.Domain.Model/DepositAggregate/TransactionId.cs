using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinExchange.Funds.Domain.Model.DepositAggregate
{
    /// <summary>
    /// Transaction ID for bitcoin transactions
    /// </summary>
    public class TransactionId
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public TransactionId()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public TransactionId(string value)
        {
            Value = value;
        }

        /// <summary>
        /// Value of the ID
        /// </summary>
        public string Value { get; set; }
    }
}

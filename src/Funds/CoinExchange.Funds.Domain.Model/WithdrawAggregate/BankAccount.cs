using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinExchange.Funds.Domain.Model.WithdrawAggregate
{
    /// <summary>
    /// Bank Account
    /// </summary>
    public class BankAccount
    {
        /// <summary>
        /// BankName
        /// </summary>
        public string BankName { get; set; }

        /// <summary>
        /// AccountAddress
        /// </summary>
        public string AccountAddress { get; set; }

        /// <summary>
        /// SwiftCode
        /// </summary>
        public string SwiftCode { get; set; }
    }
}

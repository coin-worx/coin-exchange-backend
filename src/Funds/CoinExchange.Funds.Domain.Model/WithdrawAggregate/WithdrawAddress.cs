using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Funds.Domain.Model.DepositAggregate;

namespace CoinExchange.Funds.Domain.Model.WithdrawAggregate
{
    /// <summary>
    /// Represents the Address from to withdraw from
    /// </summary>
    public class WithdrawAddress
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public WithdrawAddress(BitcoinAddress bitcoinAddress, string description, AccountId accountId)
        {
            BitcoinAddress = bitcoinAddress;
            Description = description;
            AccountId = accountId;
        }

        /// <summary>
        /// Bitcoin Address
        /// </summary>
        public BitcoinAddress BitcoinAddress { get; private set; }

        /// <summary>
        /// Unique desciption  so that the user can identify this address
        /// </summary>
        public string Description { get; private set; }

        /// <summary>
        /// AccountID
        /// </summary>
        public AccountId AccountId { get; private set; }
    }
}

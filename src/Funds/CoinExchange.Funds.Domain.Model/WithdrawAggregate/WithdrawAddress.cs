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
        public WithdrawAddress()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public WithdrawAddress(CurrencyAggregate.Currency currency, BitcoinAddress bitcoinAddress, string description, 
            AccountId accountId, DateTime creationDateTime)
        {
            Currency = currency;
            BitcoinAddress = bitcoinAddress;
            Description = description;
            AccountId = accountId;
            CreationDateTime = creationDateTime;
        }

        /// <summary>
        /// Primary key for database
        /// </summary>
        public int Id { get; private set; }

        /// <summary>
        /// Currency
        /// </summary>
        public CurrencyAggregate.Currency Currency { get; private set; }

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

        /// <summary>
        /// DateTime when this address was created
        /// </summary>
        public DateTime CreationDateTime { get; private set; }
    }
}

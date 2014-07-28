using System;
using CoinExchange.Funds.Domain.Model.CurrencyAggregate;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinExchange.Funds.Domain.Model.BalanceAggregate
{
    /// <summary>
    /// Represents an entity that has pending balance
    /// </summary>
    public class PendingTransaction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public PendingTransaction()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public PendingTransaction(string currency, string instanceId, PendingTransactionType pendingEntityType, double amount)
        {
            Currency = currency;
            InstanceId = instanceId;
            PendingTransactionType = pendingEntityType;
            Amount = amount;
        }

        /// <summary>
        /// Database Primary Key
        /// </summary>
        public string Id { get; private set; }

        /// <summary>
        /// Currency
        /// </summary>
        public string Currency { get; private set; }

        /// <summary>
        /// ID of Order or Withdrawal instance
        /// </summary>
        public string InstanceId { get; private set; }

        /// <summary>
        /// Type of the Pending Transaction i.e., Order or Withdraw
        /// </summary>
        public PendingTransactionType PendingTransactionType { get; private set; }

        /// <summary>
        /// Amount of the pending transaction
        /// </summary>
        public double Amount { get; private set; }
    }
}

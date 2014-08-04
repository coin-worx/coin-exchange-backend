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
        public PendingTransaction(Currency currency, string instanceId, PendingTransactionType pendingEntityType, 
            double amount, int balanceId)
        {
            Currency = currency;
            InstanceId = instanceId;
            PendingTransactionType = pendingEntityType;
            Amount = amount;
            BalanceId = balanceId;
        }

        /// <summary>
        /// Database Primary Key
        /// </summary>
        public int Id { get; private set; }

        /// <summary>
        /// Primary key ID of the balance database record
        /// </summary>
        public int BalanceId { get; private set; }

        /// <summary>
        /// Currency
        /// </summary>
        public virtual Currency Currency { get; private set; }

        /// <summary>
        /// ID of Order or Withdrawal instance
        /// </summary>
        public virtual string InstanceId { get; private set; }

        /// <summary>
        /// Type of the Pending Transaction i.e., Order or Withdraw
        /// </summary>
        public virtual PendingTransactionType PendingTransactionType { get; private set; }

        /// <summary>
        /// Amount of the pending transaction
        /// </summary>
        public virtual double Amount { get; private set; }
    }
}

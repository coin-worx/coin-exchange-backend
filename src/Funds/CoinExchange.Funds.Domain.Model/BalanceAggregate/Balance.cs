using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Funds.Domain.Model.CurrencyAggregate;
using CoinExchange.Funds.Domain.Model.DepositAggregate;

namespace CoinExchange.Funds.Domain.Model.BalanceAggregate
{
    /// <summary>
    /// Entity for keeping the record of balance
    /// </summary>
    public class Balance
    {
        // Get the Current Logger
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger
        (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private double _availableBalance = 0;
        private double _currentBalance = 0;
        private double _pendingBalance = 0;
        private IList<PendingTransaction> _pendingTransactions { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public Balance()
        {
            _pendingTransactions = new List<PendingTransaction>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public Balance(Currency currency, AccountId accountId)
        {
            Currency = currency;
            AccountId = accountId;
            _pendingTransactions = new List<PendingTransaction>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public Balance(Currency currency, AccountId accountId, double availableBalance, double currentBalance)
        {
            Currency = currency;
            AccountId = accountId;
            _availableBalance = availableBalance;
            _currentBalance = currentBalance;
            _pendingTransactions = new List<PendingTransaction>();
        }

        #region Methods

        /// <summary>
        /// Adds a transaction for either a withdrawal or Order that is yet pending
        /// </summary>
        /// <returns></returns>
        public bool AddPendingTransaction(string id, PendingTransactionType pendingEntityType, double amount)
        {
            // Add this pending transaction to the pending entities list. This entity can only be removed from this list 
            // when the transaction is compelte for this entity(Trade, Withdraw confirmed) or reverted in case of order
            // cancelled
            PendingTransaction pendingEntity = new PendingTransaction(Currency.Name, id, pendingEntityType, amount);
            _pendingTransactions.Add(pendingEntity);
            // Deduct the amount
            _availableBalance += amount;
            return true;
        }

        /// <summary>
        /// Called when a transaction is confirmed, updates the balance and the Pending Transaction List accordingly 
        /// </summary>
        /// <returns></returns>
        public bool ConfirmPendingTransaction(string id, PendingTransactionType pendingEntityType, double amount)
        {
            PendingTransaction pendingTransaction = GetPendingTransaction(id, pendingEntityType);
            if (pendingTransaction != null)
            {
                // As the Pending transaction is confirmed for either withdraw
                _currentBalance += amount;
            }
            
            return false;
        }

        /// <summary>
        /// Retreives the PendingTransaciton obejct from the list if present, then removes the instance from the list and 
        /// returns to the caller
        /// </summary>
        /// <param name="id"></param>
        /// <param name="pendingEntityType"></param>
        /// <returns></returns>
        private PendingTransaction GetPendingTransaction(string id, PendingTransactionType pendingEntityType)
        {
            PendingTransaction pendingTransaction = null;
            if (PendingBalance > 0)
            {
                foreach (var tempPendingTransaction in _pendingTransactions)
                {
                    if (tempPendingTransaction.InstanceId == id &&
                        tempPendingTransaction.PendingTransactionType == pendingEntityType)
                    {
                        pendingTransaction = tempPendingTransaction;
                        break;
                    }
                }
                // If the instance is found, remove it from pending transaction list and update the balance
                if (pendingTransaction != null)
                {
                    _pendingTransactions.Remove(pendingTransaction);
                    return pendingTransaction;
                }
            }
            else
            {
                Log.Error("No pending balance found for ID: " + id);
            }
            return null;
        }

        /// <summary>
        /// Adds the balance to the CurrentBalance
        /// </summary>
        /// <returns></returns>
        public bool AddCurrentBalance(double amount)
        {
            _currentBalance += amount;
            return true;
        }

        /// <summary>
        /// Adds the balance to the AvailableBalance
        /// </summary>
        /// <returns></returns>
        public bool AddAvailableBalance(double amount)
        {
            _availableBalance += amount;
            return true;
        }

        #endregion Methods

        #region Properties

        /// <summary>
        /// Primary key ID of the database
        /// </summary>
        public int Id { get; private set; }

        /// <summary>
        /// Currency
        /// </summary>
        public CurrencyAggregate.Currency Currency { get; private set; }

        /// <summary>
        /// Account Id
        /// </summary>
        public AccountId AccountId { get; private set; }

        /// <summary>
        /// The balance that is available for transaction and does not include the pending balance
        /// </summary>
        public double AvailableBalance
        {
            get { return _availableBalance; }
            private set { _availableBalance = value; }
        }

        /// <summary>
        /// The balance that includes the pending balance
        /// </summary>
        public double CurrentBalance
        {
            get { return _currentBalance; }
            private set { _currentBalance = value; }
        }

        /// <summary>
        /// The balance that is pending confirmation to be subtracted
        /// </summary>
        public double PendingBalance
        {
            get { return _currentBalance - _availableBalance; }
            private set { _pendingBalance = value; }
        }

        #endregion Properties
    }
}

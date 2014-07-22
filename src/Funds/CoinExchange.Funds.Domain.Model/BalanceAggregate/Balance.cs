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
        private double _availableBalance = 0;
        private double _currentBalance = 0;
        private double _pendingBalance = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public Balance()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public Balance(Currency currency, AccountId accountId)
        {
            Currency = currency;
            AccountId = accountId;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public Balance(Currency currency, AccountId accountId, double availableBalance, double currentBalance, double pendingBalance)
        {
            Currency = currency;
            AccountId = accountId;
            _availableBalance = availableBalance;
            _currentBalance = currentBalance;
            _pendingBalance = pendingBalance;
        }

        #region Methods

        /// <summary>
        /// Adds the balance to the Current Balance
        /// </summary>
        /// <returns></returns>
        public bool AddBalance(double amount)
        {
            AvailableBalance += amount;
            CurrentBalance += amount;
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
            get { return _pendingBalance; }
            private set { _pendingBalance = value; }
        }

        #endregion Properties
    }
}

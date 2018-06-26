/***************************************************************************** 
* Copyright 2016 Aurora Solutions 
* 
*    http://www.aurorasolutions.io 
* 
* Aurora Solutions is an innovative services and product company at 
* the forefront of the software industry, with processes and practices 
* involving Domain Driven Design(DDD), Agile methodologies to build 
* scalable, secure, reliable and high performance products.
* 
* Coin Exchange is a high performance exchange system specialized for
* Crypto currency trading. It has different general purpose uses such as
* independent deposit and withdrawal channels for Bitcoin and Litecoin,
* but can also act as a standalone exchange that can be used with
* different asset classes.
* Coin Exchange uses state of the art technologies such as ASP.NET REST API,
* AngularJS and NUnit. It also uses design patterns for complex event
* processing and handling of thousands of transactions per second, such as
* Domain Driven Designing, Disruptor Pattern and CQRS With Event Sourcing.
* 
* Licensed under the Apache License, Version 2.0 (the "License"); 
* you may not use this file except in compliance with the License. 
* You may obtain a copy of the License at 
* 
*    http://www.apache.org/licenses/LICENSE-2.0 
* 
* Unless required by applicable law or agreed to in writing, software 
* distributed under the License is distributed on an "AS IS" BASIS, 
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. 
* See the License for the specific language governing permissions and 
* limitations under the License. 
*****************************************************************************/


﻿using System;
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

        private decimal _availableBalance = 0;
        private decimal _currentBalance = 0;
        private decimal _pendingBalance = 0;
        private IList<PendingTransaction> _pendingTransactions;

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
        public Balance(Currency currency, AccountId accountId, decimal availableBalance, decimal currentBalance)
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
        public bool AddPendingTransaction(string id, PendingTransactionType pendingEntityType, decimal amount)
        {
            // Add this pending transaction to the pending entities list. This entity can only be removed from this list 
            // when the transaction is compelte for this entity(Trade, Withdraw confirmed) or reverted in case of order
            // cancelled
            PendingTransaction pendingEntity = new PendingTransaction(Currency, id, pendingEntityType, amount, BalanceId);
            _pendingTransactions.Add(pendingEntity);
            // Deduct the amount
            _availableBalance += amount;
            return true;
        }

        /// <summary>
        /// Handles the event of an order cancel, and restores amount to the available balance
        /// </summary>
        /// <param name="id"></param>
        /// <param name="pendingEntityType"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        public bool CancelPendingTransaction(string id, PendingTransactionType pendingEntityType, decimal amount)
        {
            PendingTransaction pendingTransaction = RemovePendingTransaction(id, pendingEntityType);
            if (pendingTransaction != null)
            {
                // As the Pending transaction is cancelled for order
                _availableBalance += amount;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Called when a transaction is confirmed, updates the balance and the Pending Transaction List accordingly 
        /// </summary>
        /// <returns></returns>
        public bool ConfirmPendingTransaction(string id, PendingTransactionType pendingEntityType, decimal amount)
        {
            PendingTransaction pendingTransaction = RemovePendingTransaction(id, pendingEntityType);
            if (pendingTransaction != null)
            {
                // As the Pending transaction is confirmed for either withdraw or order
                _currentBalance += amount;
                return true;
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
        private PendingTransaction RemovePendingTransaction(string id, PendingTransactionType pendingEntityType)
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
        public bool AddCurrentBalance(decimal amount)
        {
            _currentBalance += amount;
            return true;
        }

        /// <summary>
        /// Adds the balance to the AvailableBalance
        /// </summary>
        /// <returns></returns>
        public bool AddAvailableBalance(decimal amount)
        {
            _availableBalance += amount;
            return true;
        }

        /// <summary>
        /// Adds balance that cannot be added to the available or current balance at th moment, E.g., a deposit that 
        /// is over the limit
        /// </summary>
        /// <param name="amount"></param>
        /// <returns></returns>
        public bool AddPendingBalance(decimal amount)
        {
            this.PendingBalance += amount;
            return true;
        }

        /// <summary>
        /// Freezes this account balance
        /// </summary>
        /// <returns></returns>
        public bool FreezeAccount()
        {
            if (!IsFrozen)
            {
                IsFrozen = true;
                return true;
            }
            return false;
        }

        #endregion Methods

        #region Properties

        /// <summary>
        /// Primary key ID of the database
        /// </summary>
        public virtual int BalanceId { get; set; }

        /// <summary>
        /// Currency
        /// </summary>
        public virtual CurrencyAggregate.Currency Currency { get; private set; }

        /// <summary>
        /// Account Id
        /// </summary>
        public AccountId AccountId { get; private set; }

        /// <summary>
        /// The balance that is available for transaction and does not include the pending balance
        /// </summary>
        public decimal AvailableBalance
        {
            get { return _availableBalance; }
            private set { _availableBalance = value; }
        }

        /// <summary>
        /// The balance that includes the pending balance
        /// </summary>
        public decimal CurrentBalance
        {
            get { return _currentBalance; }
            private set { _currentBalance = value; }
        }

        /// <summary>
        /// The balance that is pending confirmation to be subtracted
        /// </summary>
        public decimal PendingBalance
        {
            get
            {
                return Math.Abs(_currentBalance - _availableBalance);
            }
            private set { _pendingBalance = value; }
        }

        /// <summary>
        /// PendingTransactions
        /// </summary>
        public IList<PendingTransaction> PendingTransactions
        {
            get { return _pendingTransactions; }
            set { _pendingTransactions = value; }
        }

        /// <summary>
        /// Whether this account balance is frozen or not
        /// </summary>
        public bool IsFrozen { get; private set; }

        #endregion Properties
    }
}

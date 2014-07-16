using System;
using CoinExchange.Common.Domain.Model;
using CoinExchange.Funds.Domain.Model.DepositAggregate;

namespace CoinExchange.Funds.Domain.Model.WithdrawAggregate
{
    /// <summary>
    /// Represents the Withdrawal object - Entity
    /// </summary>
    public class Withdraw
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public Withdraw()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public Withdraw(Currency currency, string withdrawId, DateTime date, string type, double amount, double fee, TransactionStatus status, AccountId accountId)
        {
            Currency = currency;
            WithdrawId = withdrawId;
            DateTime = date;
            Type = type;
            Amount = amount;
            Fee = fee;
            Status = status;
            AccountId = accountId;
        }

        /// <summary>
        /// Database primary key
        /// </summary>
        public int Id { get; private set; }

        /// <summary>
        /// Currency
        /// </summary>
        public Currency Currency { get; set; }

        /// <summary>
        /// DepositId
        /// </summary>
        public string WithdrawId { get; set; }

        /// <summary>
        /// Date
        /// </summary>
        public DateTime DateTime { get; set; }

        /// <summary>
        /// Type
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Amount
        /// </summary>
        public double Amount { get; set; }

        /// <summary>
        /// Fee
        /// </summary>
        public double Fee { get; set; }

        /// <summary>
        /// Bank Account
        /// </summary>
        public BankAccount BankAccount { get; set; }

        /// <summary>
        /// Status of the Deposit
        /// </summary>
        public TransactionStatus Status { get; set; }

        /// <summary>
        /// AccountID
        /// </summary>
        public AccountId AccountId { get; set; }

        /// <summary>
        /// Sets the status to Confirmed
        /// </summary>
        public void StatusConfirmed()
        {
            if (Status == TransactionStatus.Pending)
            {
                Status = TransactionStatus.Confirmed;
            }
        }

        /// <summary>
        /// Sets the status to Cancelled
        /// </summary>
        public void StatusCancelled()
        {
            if (Status == TransactionStatus.Pending)
            {
                Status = TransactionStatus.Cancelled;
            }
        }
    }
}

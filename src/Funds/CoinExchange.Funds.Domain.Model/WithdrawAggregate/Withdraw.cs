using System;
using CoinExchange.Common.Domain.Model;
using CoinExchange.Funds.Domain.Model.CurrencyAggregate;
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
        public Withdraw(Currency currency, string withdrawId, DateTime date, WithdrawType type, double amount, double fee, 
            TransactionStatus status, AccountId accountId, TransactionId transactionId, BitcoinAddress bitcoinAddress)
        {
            Currency = currency;
            WithdrawId = withdrawId;
            DateTime = date;
            Type = type;
            Amount = amount;
            Fee = fee;
            Status = status;
            AccountId = accountId;
            TransactionId = transactionId;
            BitcoinAddress = bitcoinAddress;
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
        public WithdrawType Type { get; set; }

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
        /// Transaction ID for the Withdrawaal
        /// </summary>
        public TransactionId TransactionId { get; set; }

        /// <summary>
        /// Bitcoin Address associated with this withdrawal
        /// </summary>
        public BitcoinAddress BitcoinAddress { get; set; }

        /// <summary>
        /// Assigns the withdraw fee to this instance
        /// </summary>
        /// <param name="fee"></param>
        /// <returns></returns>
        public bool AssignFee(double fee)
        {
            Fee = fee;
            return true;
        }

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

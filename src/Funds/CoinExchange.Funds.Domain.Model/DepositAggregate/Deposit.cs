using System;
using CoinExchange.Funds.Domain.Model.CurrencyAggregate;

namespace CoinExchange.Funds.Domain.Model.DepositAggregate
{
    /// <summary>
    /// Represents a Deposit that has been made to the user's account - Entity
    /// </summary>
    public class Deposit
    {
        /// <summary>
        /// For database record
        /// </summary>
        private int _id { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public Deposit()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public Deposit(Currency currency, string depositId, DateTime date, DepositType type, double amount, double fee,
            TransactionStatus status, AccountId accountId, TransactionId transactionId, BitcoinAddress bitcoinAddress)
        {
            Currency = currency;
            DepositId = depositId;
            Date = date;
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
        public string DepositId { get; set; }

        /// <summary>
        /// Date
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Type
        /// </summary>
        public DepositType Type { get; set; }
        
        /// <summary>
        /// Amount
        /// </summary>
        public double Amount { get; set; }

        /// <summary>
        /// Fee
        /// </summary>
        public double Fee { get; set; }

        /// <summary>
        /// Status of the Deposit
        /// </summary>
        public TransactionStatus Status { get; set; }

        /// <summary>
        /// AccountID
        /// </summary>
        public AccountId AccountId { get; set; }

        /// <summary>
        /// Transaction ID for the Deposit
        /// </summary>
        public TransactionId TransactionId { get; set; }

        /// <summary>
        /// Bitcoin Address associated with this Deposit
        /// </summary>
        public BitcoinAddress BitcoinAddress { get; set; }

        /// <summary>
        /// The number of confirmations for this Deposit
        /// </summary>
        public int Confirmations { get; private set; }

        /// <summary>
        /// Increments the Confirmations by one digit
        /// </summary>
        public void IncrementConfirmations()
        {
            Confirmations++;
        }

        /// <summary>
        /// Increments the confirmations by the given digit
        /// </summary>
        /// <param name="confirmations"></param>
        public void IncrementConfirmations(int confirmations)
        {
            Confirmations += confirmations;
            if (confirmations >= 7)
            {
                StatusConfirmed();
            }
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

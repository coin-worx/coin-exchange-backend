using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinExchange.Funds.Application.WithdrawServices.Representations
{
    /// <summary>
    /// Representation of Withdraw on application layer level
    /// </summary>
    public class WithdrawRepresentation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public WithdrawRepresentation(string currency, string withdrawId, DateTime dateTime, string type, decimal amount, decimal fee, string status)
        {
            Currency = currency;
            WithdrawId = withdrawId;
            DateTime = dateTime;
            Type = type;
            Amount = amount;
            Fee = fee;
            Status = status;
        }

        /// <summary>
        /// Currency
        /// </summary>
        public string Currency { get; private set; }

        /// <summary>
        /// DepositId
        /// </summary>
        public string WithdrawId { get; private set; }

        /// <summary>
        /// Date
        /// </summary>
        public DateTime DateTime { get; private set; }

        /// <summary>
        /// Type
        /// </summary>
        public string Type { get; private set; }

        /// <summary>
        /// Amount
        /// </summary>
        public decimal Amount { get; private set; }

        /// <summary>
        /// Fee
        /// </summary>
        public decimal Fee { get; private set; }

        /// <summary>
        /// Status of the Deposit
        /// </summary>
        public string Status { get; private set; }
    }
}

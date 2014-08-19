using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinExchange.Funds.Application.LedgerServices.Representations
{
    /// <summary>
    /// Represents the Ledger
    /// </summary>
    public class LedgerRepresentation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public LedgerRepresentation(string ledgerId, DateTime dateTime, string ledgerType, string currency, 
            decimal amount, decimal amountInUsd, decimal fee, decimal balance, string tradeId, string orderId,
            string withdrawId, string depositId, int accountId)
        {
            LedgerId = ledgerId;
            DateTime = dateTime;
            LedgerType = ledgerType;
            Currency = currency;
            Amount = amount;
            AmountInUsd = amountInUsd;
            Fee = fee;
            Balance = balance;
            TradeId = tradeId;
            OrderId = orderId;
            WithdrawId = withdrawId;
            DepositId = depositId;
            AccountId = accountId;
        }

        /// <summary>
        /// Ledger ID
        /// </summary>
        public string LedgerId { get; private set; }

        /// <summary>
        /// Datetime
        /// </summary>
        public DateTime DateTime { get; private set; }

        /// <summary>
        /// Type of Ledger
        /// </summary>
        public string LedgerType { get; private set; }

        /// <summary>
        /// Currency
        /// </summary>
        public string Currency { get; private set; }

        /// <summary>
        /// Amount
        /// </summary>
        public decimal Amount { get; private set; }

        /// <summary>
        /// Amount in US Dollars
        /// </summary>
        public decimal AmountInUsd { get; private set; }

        /// <summary>
        /// Fee
        /// </summary>
        public decimal Fee { get; private set; }

        /// <summary>
        /// Balance
        /// </summary>
        public decimal Balance { get; private set; }

        /// <summary>
        /// TradeId
        /// </summary>
        public string TradeId { get; private set; }

        /// <summary>
        /// Order ID
        /// </summary>
        public string OrderId { get; private set; }

        /// <summary>
        /// Withdraw ID
        /// </summary>
        public string WithdrawId { get; private set; }

        /// <summary>
        /// Deposit ID
        /// </summary>
        public string DepositId { get; private set; }

        /// <summary>
        /// Account ID
        /// </summary>
        public int AccountId { get; private set; }

    }
}

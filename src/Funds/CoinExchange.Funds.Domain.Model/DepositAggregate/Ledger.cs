using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Common.Domain.Model;

namespace CoinExchange.Funds.Domain.Model.DepositAggregate
{
    /// <summary>
    /// Ledger VO
    /// </summary>
    public class Ledger
    {
        public int Id { get; private set; }
        public DateTime DateTime { get; private set; }
        public LedgerType LedgerType { get; private set; }
        public Currency Currency { get; private set; }
        public decimal Amount { get; private set; }
        public decimal Fee { get; private set; }
        public decimal Balance { get; private set; }
        public string TradeId { get; private set; }
        public string OrderId { get; private set; }
        public string WithdrawId { get; private set; }
        public string DepositId { get; private set; }
        public AccountId AccountId { get; private set; }

        /// <summary>
        /// default constructor
        /// </summary>
        public Ledger()
        {
            
        }

        /// <summary>
        /// parameterized constructor
        /// </summary>
        /// <param name="id"></param>
        /// <param name="dateTime"></param>
        /// <param name="ledgerType"></param>
        /// <param name="currency"></param>
        /// <param name="amount"></param>
        /// <param name="fee"></param>
        /// <param name="balance"></param>
        /// <param name="tradeId"></param>
        /// <param name="orderId"></param>
        /// <param name="withdrawId"></param>
        /// <param name="depositId"></param>
        /// <param name="accountId"></param>
        public Ledger(int id, DateTime dateTime, LedgerType ledgerType, Currency currency, decimal amount, decimal fee, decimal balance, string tradeId, string orderId, string withdrawId, string depositId, AccountId accountId)
        {
            Id = id;
            DateTime = dateTime;
            LedgerType = ledgerType;
            Currency = currency;
            Amount = amount;
            Fee = fee;
            Balance = balance;
            TradeId = tradeId;
            OrderId = orderId;
            WithdrawId = withdrawId;
            DepositId = depositId;
            AccountId = accountId;
        }
    }
}

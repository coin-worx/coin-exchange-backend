using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Funds.Domain.Model.CurrencyAggregate;
using CoinExchange.Funds.Domain.Model.DepositAggregate;
using CoinExchange.Funds.Domain.Model.LedgerAggregate;
using CoinExchange.Funds.Domain.Model.WithdrawAggregate;

namespace CoinExchange.Funds.Domain.Model.Services
{
    /// <summary>
    /// Interface for Transaction Service
    /// </summary>
    public interface ITransactionService
    {
        /// <summary>
        /// Creates a Ledger entry for one currency of either side's of one of the trade contributors
        /// </summary>
        /// <param name="currency"></param>
        /// <param name="amount"></param>
        /// <param name="amountInUsd"> </param>
        /// <param name="fee"></param>
        /// <param name="balance"></param>
        /// <param name="executionDate"></param>
        /// <param name="orderId"></param>
        /// <param name="tradeId"></param>
        /// <param name="accountId"></param>
        /// <param name="isBaseCurrencyInTrade"></param>
        /// <returns></returns>
        bool CreateLedgerEntry(Currency currency, decimal amount, decimal amountInUsd, decimal fee, decimal balance,
            DateTime executionDate, string orderId, string tradeId, AccountId accountId, bool isBaseCurrencyInTrade);

        bool CreateDepositTransaction(Deposit deposit, decimal balance);

        bool CreateWithdrawTransaction(Withdraw withdraw, decimal balance);
    }
}

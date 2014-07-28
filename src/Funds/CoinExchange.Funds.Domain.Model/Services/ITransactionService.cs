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
        bool CreateLedgerEntry(Currency currency, double amount, double fee, double balance, DateTime executionDate,
                               string orderId, string tradeId, AccountId accountId);

        bool CreateTradeTransaction(string currencyPair, double tradeVolume, double price,
                                    DateTime executionDateTime, string tradeId, string buyAccountId, string sellAccountId,
                                    string buyOrderId, string sellOrderId);

        bool CreateDepositTransaction(Deposit deposit, double balance);

        bool CreateWithdrawTransaction(Withdraw withdraw, double balance);
    }
}

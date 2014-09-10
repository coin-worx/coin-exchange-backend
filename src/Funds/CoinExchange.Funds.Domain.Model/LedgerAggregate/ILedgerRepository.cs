using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Funds.Domain.Model.DepositAggregate;

namespace CoinExchange.Funds.Domain.Model.LedgerAggregate
{
    /// <summary>
    /// Interface for Ledger Repository
    /// </summary>
    public interface ILedgerRepository
    {
        Ledger GetLedgerById(int id);
        List<Ledger> GetLedgerByAccountId(AccountId accountId);
        List<Ledger> GetLedgerByCurrencyName(string currency);
        IList<Ledger> GetLedgerByAccountIdAndCurrency(string currency, AccountId accountId);
        Ledger GetLedgerByLedgerId(string ledgerId);
        List<Ledger> GetLedgersByTradeId(string tradeId);
        Ledger GetLedgersByDepositId(string depositId);
        Ledger GetLedgersByWithdrawId(string withdrawId);
        List<Ledger> GetLedgersByOrderId(string orderId);
        decimal GetBalanceForCurrency(string currency, AccountId accountId);
        IList<Ledger> GetAllLedgers(int accountId);
        IList<Ledger> GetAllLedgers();
    }
}

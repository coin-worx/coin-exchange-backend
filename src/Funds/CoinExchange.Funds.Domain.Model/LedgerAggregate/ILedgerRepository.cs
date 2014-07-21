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
        Ledger GetLedgerByLedgerId(string ledgerId);
        List<Ledger> GetLedgersByTradeId(string tradeId);
        List<Ledger> GetLedgersByDepositId(string depositId);
        List<Ledger> GetLedgersByWithdrawId(string withdrawId);
        List<Ledger> GetLedgersByOrderId(string orderId);
        double GetBalanceForCurrency(string currency, AccountId accountId);
        IList<Ledger> GetAllLedgers();
    }
}

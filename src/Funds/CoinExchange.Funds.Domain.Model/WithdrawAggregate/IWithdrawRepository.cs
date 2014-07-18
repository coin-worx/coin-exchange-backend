using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Funds.Domain.Model.DepositAggregate;

namespace CoinExchange.Funds.Domain.Model.WithdrawAggregate
{
    /// <summary>
    /// Interface for Withdraw Repository
    /// </summary>
    public interface IWithdrawRepository
    {
        Withdraw GetWithdrawById(int id);
        List<Withdraw> GetWithdrawByAccountId(AccountId accountId);
        List<Withdraw> GetWithdrawByCurrencyName(string currency);
        Withdraw GetWithdrawByWithdrawId(string withdrawId);
        Withdraw GetWithdrawByTransactionId(TransactionId transactionId);
        List<Withdraw> GetWithdrawByBitcoinAddress(BitcoinAddress bitcoinAddress);
    }
}

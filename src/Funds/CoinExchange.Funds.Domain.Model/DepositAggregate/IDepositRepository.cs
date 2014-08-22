using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinExchange.Funds.Domain.Model.DepositAggregate
{
    /// <summary>
    /// Interface for accessing Deposit objects from repository
    /// </summary>
    public interface IDepositRepository
    {
        Deposit GetDepositById(int id);
        List<Deposit> GetDepositByAccountId(AccountId accountId);
        List<Deposit> GetDepositByCurrencyName(string currency);
        Deposit GetDepositByDepositId(string depositId);
        Deposit GetDepositByTransactionId(TransactionId transactionId);
        Deposit GetDepositsByBitcoinAddress(BitcoinAddress bitcoinAddress);
        List<Deposit> GetAllDeposits();
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Funds.Domain.Model.DepositAggregate;

namespace CoinExchange.Funds.Domain.Model.BalanceAggregate
{
    /// <summary>
    /// Interface for getting the balance for a currency
    /// </summary>
    public interface IBalanceRepository
    {
        /// <summary>
        /// Gets the balance given the ID primary key of the database
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Balance GetBalanceById(int id);

        /// <summary>
        /// Returnss the balance object when provided the currency and the account ID
        /// </summary>
        /// <param name="?"></param>
        /// <param name="currency"> </param>
        /// <param name="accountId"></param>
        /// <returns></returns>
        Balance GetBalanceByCurrencyAndAccountId(CurrencyAggregate.Currency currency, AccountId accountId);
    }
}

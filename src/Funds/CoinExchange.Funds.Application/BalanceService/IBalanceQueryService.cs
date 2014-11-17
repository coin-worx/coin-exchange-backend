using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Funds.Application.BalanceService.Representations;
using CoinExchange.Funds.Domain.Model.DepositAggregate;

namespace CoinExchange.Funds.Application.BalanceService
{
    /// <summary>
    /// Provides basic mehtods for querying balance details
    /// </summary>
    public interface IBalanceQueryService
    {
        /// <summary>
        /// Get accound id balances
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns></returns>
        List<BalanceDetails> GetBalances(AccountId accountId);
    }
}

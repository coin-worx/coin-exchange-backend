using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Funds.Application.BalanceService.Representations;
using CoinExchange.Funds.Domain.Model.BalanceAggregate;
using CoinExchange.Funds.Domain.Model.DepositAggregate;

namespace CoinExchange.Funds.Application.BalanceService
{
    /// <summary>
    /// Implementation of balance query serivce
    /// </summary>
    public class BalanceQueryService:IBalanceQueryService
    {
        private IBalanceRepository _balanceRepository;

        public BalanceQueryService(IBalanceRepository balanceRepository)
        {
            _balanceRepository = balanceRepository;
        }

        /// <summary>
        /// Get balances of all the currency pair of an account id
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns></returns>
        public List<BalanceDetails> GetBalances(AccountId accountId)
        {
            List<BalanceDetails> balanceDetails=new List<BalanceDetails>();
            List<Balance> balances = _balanceRepository.GetAllCurrienceBalances(accountId);
            for (int i = 0; i < balances.Count; i++)
            {
                BalanceDetails details=new BalanceDetails(balances[i].Currency.Name,balances[i].AvailableBalance);
                balanceDetails.Add(details);
            }
            return balanceDetails;
        }
    }
}

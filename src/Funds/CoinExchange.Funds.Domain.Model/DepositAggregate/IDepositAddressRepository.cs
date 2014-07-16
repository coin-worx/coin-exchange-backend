using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinExchange.Funds.Domain.Model.DepositAggregate
{
    /// <summary>
    /// Interface for Deposit Address Repository
    /// </summary>
    public interface IDepositAddressRepository
    {
        DepositAddress GetDepositAddressById(int id);
        List<DepositAddress> GetDepositAddressByAccountId(AccountId accountId);
    }
}

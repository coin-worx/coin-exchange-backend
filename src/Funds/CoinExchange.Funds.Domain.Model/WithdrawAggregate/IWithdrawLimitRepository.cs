using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinExchange.Funds.Domain.Model.WithdrawAggregate
{
    /// <summary>
    /// Interface for Withdrawa Repository
    /// </summary>
    public interface IWithdrawLimitRepository
    {
        /// <summary>
        /// Gets the Withdraw limit by specifying the Tier Level
        /// </summary>
        /// <param name="tierLevel"></param>
        /// <returns></returns>
        WithdrawLimit GetWithdrawLimitByTierLevel(string tierLevel);

        /// <summary>
        /// Gets the withdraw limit by specifying the database primary key
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        WithdrawLimit GetWithdrawLimitById(int id);
    }
}

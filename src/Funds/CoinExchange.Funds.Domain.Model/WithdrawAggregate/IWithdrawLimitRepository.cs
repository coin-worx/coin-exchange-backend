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
        /// Gets the Withdraw Limit by Tier Level and Currency Type
        /// </summary>
        /// <param name="tierLevel"></param>
        /// <param name="currencyType"></param>
        /// <returns></returns>
        WithdrawLimit GetLimitByTierLevelAndCurrency(string tierLevel, string currencyType);

        /// <summary>
        /// Gets the withdraw limit by specifying the database primary key
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        WithdrawLimit GetWithdrawLimitById(int id);

        /// <summary>
        /// Get all the withdrawals
        /// </summary>
        /// <returns></returns>
        IList<WithdrawLimit> GetAllWithdrawLimits();
    }
}

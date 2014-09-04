using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinExchange.Funds.Domain.Model.DepositAggregate
{
    /// <summary>
    /// Interface for Deposit Limit repository
    /// </summary>
    public interface IDepositLimitRepository
    {
        /// <summary>
        /// Gets the Deposit limit by specifying the Tier Level
        /// </summary>
        /// <param name="tierLevel"></param>
        /// <returns></returns>
        DepositLimit GetDepositLimitByTierLevel(string tierLevel);

        /// <summary>
        /// Gets the Deposit limit by specifying the database primary key
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        DepositLimit GetDepositLimitById(int id);

        /// <summary>
        /// Get all the deposit limits master data
        /// </summary>
        /// <returns></returns>
        IList<DepositLimit> GetAllDepositLimits();
    }
}

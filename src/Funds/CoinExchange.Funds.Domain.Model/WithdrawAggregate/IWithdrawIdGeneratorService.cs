using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinExchange.Funds.Domain.Model.WithdrawAggregate
{
    /// <summary>
    /// Interface for creating a new IDs for Withdraw instances
    /// </summary>
    public interface IWithdrawIdGeneratorService
    {
        /// <summary>
        /// Generates new ID
        /// </summary>
        /// <returns></returns>
        string GenerateNewId();
    }
}

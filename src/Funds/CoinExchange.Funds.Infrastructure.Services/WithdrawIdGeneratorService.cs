using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Funds.Domain.Model.WithdrawAggregate;

namespace CoinExchange.Funds.Infrastructure.Services
{
    /// <summary>
    /// Service for generating the IDs for withdraw instances
    /// </summary>
    public class WithdrawIdGeneratorService : IWithdrawIdGeneratorService
    {
        /// <summary>
        /// Returns new ID for a withdraw instance
        /// </summary>
        /// <returns></returns>
        public string GenerateNewId()
        {
            return Guid.NewGuid().ToString();
        }
    }
}

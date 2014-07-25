using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Funds.Domain.Model.DepositAggregate;

namespace CoinExchange.Funds.Infrastructure.Services
{
    /// <summary>
    /// Generates a new ID for Deposits
    /// </summary>
    public class DepositIdGeneratorService : IDepositIdGeneratorService
    {
        /// <summary>
        /// Returns new DepositID
        /// </summary>
        /// <returns></returns>
        public string GenerateId()
        {
            return Guid.NewGuid().ToString();
        }
    }
}

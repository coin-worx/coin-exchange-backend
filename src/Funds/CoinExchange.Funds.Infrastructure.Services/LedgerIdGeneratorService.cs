using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Funds.Domain.Model.Services;

namespace CoinExchange.Funds.Infrastructure.Services
{
    /// <summary>
    /// Generates a new Unique ID for a new ledger
    /// </summary>
    public class LedgerIdGeneratorService : ILedgerIdGeneraterService
    {
        /// <summary>
        /// Genrates a new uniqe ID
        /// </summary>
        /// <returns></returns>
        public string GenerateLedgerId()
        {
            return Guid.NewGuid().ToString();
        }
    }
}

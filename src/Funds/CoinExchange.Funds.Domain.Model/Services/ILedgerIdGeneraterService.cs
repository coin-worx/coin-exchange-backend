using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinExchange.Funds.Domain.Model.Services
{
    /// <summary>
    /// Generates a new unique ID for Ledger
    /// </summary>
    public interface ILedgerIdGeneraterService
    {
        string GenerateLedgerId();
    }
}

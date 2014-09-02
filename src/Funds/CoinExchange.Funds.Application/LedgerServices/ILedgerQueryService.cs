using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Funds.Application.LedgerServices.Representations;

namespace CoinExchange.Funds.Application.LedgerServices
{
    /// <summary>
    /// Interface for querying Ledgers
    /// </summary>
    public interface ILedgerQueryService
    {
        /// <summary>
        /// Gets all the Ledgers for the given Account ID and currency
        /// </summary>
        /// <returns></returns>
        IList<LedgerRepresentation> GetAllLedgers(int acocuntId, string currency);

        /// <summary>
        /// Get the details for the given ledger ID
        /// </summary>
        /// <param name="ledgerId"></param>
        /// <returns></returns>
        LedgerRepresentation GetLedgerDetails(string ledgerId);
    }
}

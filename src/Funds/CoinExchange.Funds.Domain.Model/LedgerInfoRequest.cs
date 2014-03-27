using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
 * Author: Waqas
 * Comany: Aurora Solutions
 */

namespace CoinExchange.Funds.Domain.Model
{
    /// <summary>
    /// The request object for the LedgerInfo
    /// </summary>
    public class LedgerInfoRequest
    {
        /// <summary>
        /// Asset Class e.g., Currency etc.
        /// </summary>
        public string AssetClass { get; set; }

        /// <summary>
        /// Asset e.g., XBT, USD etc
        /// </summary>
        public string Asset { get; set; }

        /// <summary>
        /// Type of Ledger, e.g., Trade etc
        /// </summary>
        public string Type { get; set; }
    }
}

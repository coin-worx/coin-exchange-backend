using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinExchange.Funds.Domain.Model.Entities
{
    /// <summary>
    /// Trade Balance
    /// </summary>
    public class TradeBalance
    {
        /// <summary>
        /// Asset e.g., LTC
        /// </summary>
        public string Asset { get; set; }

        /// <summary>
        /// Cumulative Trade Balance
        /// </summary>
        public decimal Balance { get; set; }

        // ToDo: Addtiional fields need to be added later while developing the domain model. https://www.kraken.com/help/api#get-trade-balance
    }
}

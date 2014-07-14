using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinExchange.Common.Domain.Model
{
    /// <summary>
    /// Represents a single currency - VO
    /// </summary>
    public class Currency
    {
        /// <summary>
        /// Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Is the currency a crypto currency(XRP, XBT, LTC etc.) or a fiat currency(USD, EUR etc.)
        /// </summary>
        public bool IsCryptoCurrency { get; set; }
    }
}

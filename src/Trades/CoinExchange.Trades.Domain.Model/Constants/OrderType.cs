using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinExchange.Trades.Domain.Model.Constants
{
    /// <summary>
    /// serves the purpose of order type comparisons with inputs
    /// </summary>
    public static class OrderType
    {
        public const string LimitOrder = "limit";
        public const string MarketOrder = "market";
    }
}

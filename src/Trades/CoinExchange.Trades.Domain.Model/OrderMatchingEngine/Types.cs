using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinExchange.Trades.Domain.Model.OrderMatchingEngine
{
    /// <summary>
    /// Contains the values to be used as constants
    /// </summary>
    public class ConstantTypes
    {
        public const decimal MARKET_ORDER_PRICE = 0;

        public const decimal MARKET_ORDER_BID_SORT_PRICE = -1;

        public const decimal MARKET_ORDER_ASK_SORT_PRICE = 0;

        public const decimal PRICE_UNCHANGED = 0;

        public const decimal SIZE_UNCHANGED = 0;
    }
}

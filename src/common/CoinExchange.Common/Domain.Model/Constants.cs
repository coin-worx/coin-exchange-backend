using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinExchange.Common.Domain.Model
{
    /// <summary>
    /// serves for the purpose of constants throug out the domain
    /// </summary>
    public static class Constants
    {
        public const string ORDER_TYPE_LIMIT = "limit";
        public const string ORDER_TYPE_MARKET = "market";
        public const string ORDER_SIDE_BUY = "buy";
        public const string ORDER_SIDE_SELL = "sell";
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Trades.Domain.Model.Entities;

namespace CoinExchange.Trades.Infrastructure.Services.Services
{
    /// <summary>
    /// Service serving the iperations relatedto Market Data
    /// </summary>
    public class MarketDataService
    {
        /// <summary>
        /// Returns the Order Book
        /// </summary>
        /// <returns></returns>
        public List<object> GetOrderBook(string symbol)
        {
            List<object> list = new List<object>();
            list.Add(symbol);
            list.Add("asks");
            list.Add(new object[] { "23", "1000", "204832014" });
            list.Add(new object[] { "32", "1000", "204832014" });
            list.Add(new object[] { "34", "1000", "204832014" });

            list.Add("bids");
            list.Add(new object[] { "23", "1000", "204832014" });
            list.Add(new object[] { "23", "1000", "204832014" });
            list.Add(new object[] { "23", "1000", "204832014" });

            return list;
        } 
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Funds.Domain.Model.Services;

namespace CoinExchange.Funds.Infrastructure.Services
{
    /// <summary>
    /// Gets the best bid and best ask
    /// </summary>
    public class StubBboCrossContextService : IBboCrossContextService
    {
        /// <summary>
        /// Gets the best bid and best ask. Item 1 = Best Bid, Item 2 = Best Ask
        /// </summary>
        /// <param name="baseCurrency"></param>
        /// <param name="quoteCurrency"></param>
        /// <returns></returns>
        public Tuple<decimal, decimal> GetBestBidBestAsk(string baseCurrency, string quoteCurrency)
        {
            return new Tuple<decimal, decimal>(580.65M, 590.87M); 
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinExchange.Funds.Domain.Model.Services
{
    /// <summary>
    /// Retrieves the Best Bid and Best Ask from the Trades BC
    /// </summary>
    public interface IBboRetrievalService
    {
        /// <summary>
        /// Gets best bid and best ask. Item 1 = Best Bid, Item 2 = Best Ask
        /// </summary>
        /// <returns></returns>
        Tuple<decimal, decimal> GetBestBidBestAsk(string baseCurrency, string quoteCurrency);
    }
}

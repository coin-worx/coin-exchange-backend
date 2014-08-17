using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Funds.Domain.Model.Services;

namespace CoinExchange.Funds.Infrastructure.Services
{
    /// <summary>
    /// Gets the best bid and best ask from the Trades BC
    /// </summary>
    public class BboCrossContextService : IBboCrossContextService
    {
        private dynamic _marketDataQueryService = null;

        /// <summary>
        /// Default Constructor
        /// </summary>
        public BboCrossContextService(dynamic marketDataQueryService)
        {
            _marketDataQueryService = marketDataQueryService;
        }

        /// <summary>
        /// Gets the Best Bid and Best Ask for a particular Currency
        /// </summary>
        /// <param name="baseCurrency"></param>
        /// <param name="quoteCurrency"></param>
        /// <returns></returns>
        public Tuple<decimal, decimal> GetBestBidBestAsk(string baseCurrency, string quoteCurrency)
        {
            dynamic bbo = _marketDataQueryService.GetBBO(baseCurrency + quoteCurrency);
            if (bbo != null)
            {
                return new Tuple<decimal, decimal>(bbo.BestBidPrice, bbo.BestAskPrice);
            }
            throw new NullReferenceException("Best Bid and Ask could not be obtained from Trades BC");
        }
    }
}

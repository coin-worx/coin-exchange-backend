using System;
using System.Collections.Generic;
using System.Web.Http;
using CoinExchange.Trades.Infrastructure.Services.Services;

namespace CoinExchange.Trades.Port.Adapter.RestService
{
    /// <summary>
    /// Serves the RESTful calls related to Market Data
    /// </summary>
    public class MarketDataController : ApiController
    {
        private MarketDataService _marketDataService = null;

        /// <summary>
        /// Default constructor
        /// </summary>
        public MarketDataController()
        {
            _marketDataService = new MarketDataService();
        }

        /// <summary>
        /// Returns orders that have not been executed but those that have been accepted on the server. Exception can be 
        /// provided in the second parameter
        /// Params:
        /// 1. TraderId(ValueObject)[FromBody]: Contains an Id of the trader, used for authentication of the trader
        /// 2. includeTrades(bool): Include trades as well in the response(optional)
        /// 3. userRefId: Restrict results to given user reference id (optional)
        /// </summary>
        /// <returns></returns>
        //[Route("marketdata/orderbook")]
        [HttpGet]
        public IHttpActionResult OpenOrderList(string currencyPair, int count = 0)
        {
            try
            {
                List<object> list = _marketDataService.GetOrderBook(currencyPair);
                if (list != null)
                {
                    return Ok<List<object>>(list);
                }
                return NotFound();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}

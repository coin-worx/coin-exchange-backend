using System;
using System.Collections.Generic;
using System.Web.Http;
using CoinExchange.Trades.Application.MarketDataServices;

namespace CoinExchange.Trades.Port.Adapter.Rest.Resources
{
    /// <summary>
    /// Market Data Service class rest expose
    /// </summary>
    public class MarketController : ApiController
    {
        private IMarketDataApplicationService _marketDataService;

        /// <summary>
        /// Default constructor
        /// </summary>
        public MarketController(IMarketDataApplicationService marketDataApplicationService)
        {
            _marketDataService = marketDataApplicationService;
        }

        [HttpGet]
        [Route("marketdata/tickerinfo")]
        public IHttpActionResult TickerInfo(string currencyPair)
        {
            try
            {
                return Ok(_marketDataService.GetTickerInfo(currencyPair));
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [Route("marketdata/ohlcinfo")]
        public IHttpActionResult OhlcInfo(string currencyPair, int interval = 1, string since = "")
        {
            try
            {
                return Ok(_marketDataService.GetOhlcInfo(currencyPair,interval, since));
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Public call that returns the Orders for a particular currency pair
        /// Params:
        /// 1. currencyPair (Required)
        /// 2. Count(optional)
        /// </summary>
        /// <returns></returns>
        [Route("marketdata/orderbook")]
        [HttpGet]
        public IHttpActionResult GetOrderBook(string currencyPair, int count = 0)
        {
            try
            {
                List<object> list = _marketDataService.GetOrderBook(currencyPair, count);
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

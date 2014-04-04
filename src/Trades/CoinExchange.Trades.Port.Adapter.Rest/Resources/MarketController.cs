using System;
using System.Collections.Generic;
using System.Web.Http;
using CoinExchange.Trades.Application.MarketData;

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
        [Route("marketData/tickerinfo")]
        public IHttpActionResult TickerInfo(string pair)
        {
            try
            {
                return Ok(_marketDataService.GetTickerInfo(pair));

            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }

        }

        [HttpGet]
        [Route("marketData/ohlcinfo")]
        public IHttpActionResult OhlcInfo(string pair, int interval = 1, string since = "")
        {
            try
            {
                return Ok(_marketDataService.GetOhlcInfo(pair,interval, since));
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

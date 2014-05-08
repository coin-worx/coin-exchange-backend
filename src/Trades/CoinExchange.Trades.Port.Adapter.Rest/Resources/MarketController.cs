using System;
using System.Collections.Generic;
using System.Web.Http;
using CoinExchange.Trades.Application.MarketDataServices;
using CoinExchange.Trades.ReadModel.MemoryImages;

namespace CoinExchange.Trades.Port.Adapter.Rest.Resources
{
    /// <summary>
    /// Market Data Service class rest expose
    /// </summary>
    public class MarketController : ApiController
    {
        private IMarketDataQueryService _marketDataService;

        /// <summary>
        /// Default constructor
        /// </summary>
        public MarketController(IMarketDataQueryService marketDataQueryService)
        {
            _marketDataService = marketDataQueryService;
        }

        /// <summary>
        /// Ticker Information
        /// </summary>
        /// <param name="currencyPair"></param>
        /// <returns></returns>
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

        /// <summary>
        /// OHLC information
        /// </summary>
        /// <param name="currencyPair"></param>
        /// <param name="interval"></param>
        /// <param name="since"></param>
        /// <returns></returns>
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
                Tuple<OrderRepresentationList, OrderRepresentationList> list = _marketDataService.GetOrderBook(currencyPair, count);
                if (list != null)
                {
                    return Ok<Tuple<OrderRepresentationList, OrderRepresentationList>>(list);
                }
                return NotFound();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Returns the Depth in IHttpActionresult as a Tuple where 
        /// Item1 = BidDepth,
        /// Item2 = AskDepth
        /// Each is an array of a Tuple of <decimal, decimal, int>, representing Volume, Price and OrderCount respectively
        /// </summary>
        /// <param name="currencyPair"></param>
        /// <returns></returns>
        [Route("marketdata/depth")]
        [HttpGet]
        public IHttpActionResult GetDepth(string currencyPair)
        {
            try
            {
                Tuple<Tuple<decimal, decimal, int>[], Tuple<decimal, decimal, int>[]> depth = _marketDataService.GetDepth(currencyPair);

                if (depth != null)
                {
                    return Ok(depth);
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

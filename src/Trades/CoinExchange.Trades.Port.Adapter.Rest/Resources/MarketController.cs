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
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger
            (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
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
            if (log.IsDebugEnabled)
            {
                log.Debug("Ticker Info Call: Currency Pair:"+currencyPair);
            }
            try
            {
                return Ok(_marketDataService.GetTickerInfo(currencyPair));
            }
            catch (Exception exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("Ticker Info Error",exception);
                }
                return InternalServerError(exception);
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
            if (log.IsDebugEnabled)
            {
                log.Debug("Ohlc Info Call: Currency Pair="+currencyPair);
            }
            try
            {
                return Ok(_marketDataService.GetOhlcInfo(currencyPair,interval, since));
            }
            catch (Exception exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("Ohlc Info Error", exception);
                }
                return InternalServerError(exception);
            }
        }

        /// <summary>
        /// Public call to return the Rate for a particular currency
        /// </summary>
        /// <returns></returns>
        [Route("marketdata/rate")]
        [HttpGet]
        public IHttpActionResult GetRate(string currencyPair)
        {
            if (log.IsDebugEnabled)
            {
                log.Debug("Get Rate Call: Currency Pair = " + currencyPair);
            }
            try
            {
                Rate rate = _marketDataService.GetRate(currencyPair);
                if (rate != null)
                {
                    return Ok<Rate>(rate);
                }
                return BadRequest();
            }
            catch (Exception exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("Get Rate Call Error", exception);
                }
                return InternalServerError(exception);
            }
        }

        [Route("marketdata/rates")]
        [HttpGet]
        public IHttpActionResult GetAllRates()
        {
            if (log.IsDebugEnabled)
            {
                log.Debug("Get All Rates Call");
            }
            try
            {
                RatesList rates = _marketDataService.GetAllRates();
                if (rates != null)
                {
                    return Ok<RatesList>(rates);
                }
                return BadRequest();
            }
            catch (Exception exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("Get All Rates Call Error", exception);
                }
                return InternalServerError(exception);
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
            if (log.IsDebugEnabled)
            {
                log.Debug("Get Order Book Call: Currency Pair=" + currencyPair);
            }
            try
            {
                Tuple<OrderRepresentationList, OrderRepresentationList> list = _marketDataService.GetOrderBook(currencyPair, count);

                if (list != null)
                {
                    return Ok<Tuple<OrderRepresentationList, OrderRepresentationList>>(list);
                }
                return BadRequest();
            }
            catch (Exception exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("Get Order Book Call Error", exception);
                }
                return InternalServerError(exception);
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
            if (log.IsDebugEnabled)
            {
                log.Debug("Get Depth Call: Currency Pair=" + currencyPair);
            }
            try
            {
                Tuple<Tuple<decimal, decimal, int>[], Tuple<decimal, decimal, int>[]> depth = _marketDataService.GetDepth(currencyPair);

                if (depth != null)
                {
                    return Ok(depth);
                }
                return BadRequest();
            }
            catch (Exception exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("Get Depth Call Error", exception);
                }
                return InternalServerError(exception);
            }
        }
    }
}

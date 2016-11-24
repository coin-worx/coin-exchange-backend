/***************************************************************************** 
* Copyright 2016 Aurora Solutions 
* 
*    http://www.aurorasolutions.io 
* 
* Aurora Solutions is an innovative services and product company at 
* the forefront of the software industry, with processes and practices 
* involving Domain Driven Design(DDD), Agile methodologies to build 
* scalable, secure, reliable and high performance products.
* 
* Coin Exchange is a high performance exchange system specialized for
* Crypto currency trading. It has different general purpose uses such as
* independent deposit and withdrawal channels for Bitcoin and Litecoin,
* but can also act as a standalone exchange that can be used with
* different asset classes.
* Coin Exchange uses state of the art technologies such as ASP.NET REST API,
* AngularJS and NUnit. It also uses design patterns for complex event
* processing and handling of thousands of transactions per second, such as
* Domain Driven Designing, Disruptor Pattern and CQRS With Event Sourcing.
* 
* Licensed under the Apache License, Version 2.0 (the "License"); 
* you may not use this file except in compliance with the License. 
* You may obtain a copy of the License at 
* 
*    http://www.apache.org/licenses/LICENSE-2.0 
* 
* Unless required by applicable law or agreed to in writing, software 
* distributed under the License is distributed on an "AS IS" BASIS, 
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. 
* See the License for the specific language governing permissions and 
* limitations under the License. 
*****************************************************************************/


﻿using System;
using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Description;
using CoinExchange.Common.Domain.Model;
using CoinExchange.Trades.Application.MarketDataServices;
using CoinExchange.Trades.Application.MarketDataServices.Representation;
using CoinExchange.Trades.ReadModel.DTO;
using CoinExchange.Trades.ReadModel.MemoryImages;

namespace CoinExchange.Trades.Port.Adapter.Rest.Resources
{
    /// <summary>
    /// Market Data Service class rest expose
    /// </summary>
    [RoutePrefix("v1")]
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
        [ResponseType(typeof(List<TickerInfoReadModel>))]
        public IHttpActionResult TickerInfo(string currencyPair)
        {
            if (log.IsDebugEnabled)
            {
                log.Debug("Ticker Info Call: Currency Pair:"+currencyPair);
            }
            try
            {
                AssertionConcern.AssertNullOrEmptyString(currencyPair, "CurrencyPair cannot be null or empty.");
                return Ok(_marketDataService.GetTickerInfo(currencyPair));
            }
            catch (ArgumentNullException exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("Ticker Info Error", exception);
                }
                return BadRequest(exception.Message);
            }
            catch (Exception exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("Ticker Info Error",exception);
                }
                return InternalServerError();
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
        [ResponseType(typeof(OhlcRepresentation))]
        public IHttpActionResult OhlcInfo(string currencyPair, int interval = 1, string since = "")
        {
            if (log.IsDebugEnabled)
            {
                log.Debug("Ohlc Info Call: Currency Pair="+currencyPair);
            }
            try
            {
                AssertionConcern.AssertNullOrEmptyString(currencyPair, "CurrencyPair cannot be null or empty.");
                return Ok(_marketDataService.GetOhlcInfo(currencyPair,interval, since));
            }
            catch (ArgumentNullException exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("Ohlc Info Error", exception);
                }
                return BadRequest(exception.Message);
            }
            catch (Exception exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("Ohlc Info Error", exception);
                }
                return InternalServerError();
            }
        }

        /// <summary>
        /// Public call to return the Rate for a particular currency
        /// </summary>
        /// <returns></returns>
        [Route("marketdata/rate")]
        [HttpGet]
        [ResponseType(typeof(Rate))]
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
                return BadRequest("Invalid currency pair or currency pair not specified.");
            }
            catch (Exception exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("Get Rate Call Error", exception);
                }
                return InternalServerError();
            }
        }

        [Route("marketdata/rates")]
        [HttpGet]
        [ResponseType(typeof(RatesList))]
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
                return InternalServerError();
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
        [ResponseType(typeof(OrderBookRepresentation))]
        public IHttpActionResult GetOrderBook(string currencyPair, int count = 0)
        {
            if (log.IsDebugEnabled)
            {
                log.Debug("Get Order Book Call: Currency Pair=" + currencyPair);
            }
            try
            {
                var list = _marketDataService.GetOrderBook(currencyPair, count);

                if (list != null)
                {
                    return Ok(list);
                }
                return BadRequest();
            }
            catch (Exception exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("Get Order Book Call Error", exception);
                }
                return InternalServerError();
            }
        }

        /// <summary>
        /// get bids asks spread.
        /// </summary>
        /// <param name="currencyPair"></param>
        /// <returns></returns>
        [Route("marketdata/spread")]
        [HttpGet]
        [ResponseType(typeof(List<Spread>))]
        public IHttpActionResult GetSpread(string currencyPair)
        {
            if (log.IsDebugEnabled)
            {
                log.Debug("Get Spread Call: Currency Pair=" + currencyPair);
            }
            try
            {
                AssertionConcern.AssertNullOrEmptyString(currencyPair, "CurrencyPair cannot be null or empty.");
                var list = _marketDataService.GetSpread(currencyPair);

                if (list != null)
                {
                    return Ok(list);
                }
                return BadRequest();
            }
            catch (ArgumentNullException exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("Get Spread Call Error", exception);
                }
                return BadRequest(exception.Message);
            }
            catch (Exception exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("Get Spread Call Error", exception);
                }
                return InternalServerError();
            }
        }

        /// <summary>
        /// get bids asks spread.
        /// </summary>
        /// <param name="currencyPair"></param>
        /// <returns></returns>
        [Route("marketdata/bbo")]
        [HttpGet]
        [ResponseType(typeof(BBORepresentation))]
        public IHttpActionResult GetBbo(string currencyPair)
        {
            if (log.IsDebugEnabled)
            {
                log.Debug("Get Bbo Call: Currency Pair=" + currencyPair);
            }
            try
            {
                AssertionConcern.AssertNullOrEmptyString(currencyPair, "CurrencyPair cannot be null or empty.");
                var list = _marketDataService.GetBBO(currencyPair);

                if (list != null)
                {
                    return Ok(list);
                }
                return BadRequest();
            }
            catch (ArgumentNullException exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("Get Bbo Call Error", exception);
                }
                return BadRequest(exception.Message);
            }
            catch (Exception exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("Get Bbo Call Error", exception);
                }
                return InternalServerError();
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
        [ResponseType(typeof(DepthTupleRepresentation))]
        public IHttpActionResult GetDepth(string currencyPair)
        {
            if (log.IsDebugEnabled)
            {
                log.Debug("Get Depth Call: Currency Pair=" + currencyPair);
            }
            try
            {
                AssertionConcern.AssertNullOrEmptyString(currencyPair, "CurrencyPair cannot be null or empty.");
                var depth = _marketDataService.GetDepth(currencyPair);

                if (depth != null)
                {
                    return Ok(depth);
                }
                return BadRequest();
            }
            catch (ArgumentNullException exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("Get Depth Call Error", exception);
                }
                return BadRequest(exception.Message);
            }
            catch (Exception exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("Get Depth Call Error", exception);
                }
                return InternalServerError();
            }
        }
    }
}

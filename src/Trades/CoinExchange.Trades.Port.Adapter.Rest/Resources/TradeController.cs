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
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;
using CoinExchange.Common.Domain.Model;
using CoinExchange.Common.Services;
using CoinExchange.Trades.Application.OrderServices.Representation;
using CoinExchange.Trades.Application.TradeServices;
using CoinExchange.Trades.Application.TradeServices.Representation;
using CoinExchange.Trades.Domain.Model.CurrencyPairAggregate;
using CoinExchange.Trades.Domain.Model.OrderAggregate;
using CoinExchange.Trades.Domain.Model.TradeAggregate;
using CoinExchange.Trades.Port.Adapter.Rest.DTOs;
using CoinExchange.Trades.Port.Adapter.Rest.DTOs.Trade;
using CoinExchange.Trades.ReadModel.DTO;
using Spring.Context;
using Spring.Context.Support;

namespace CoinExchange.Trades.Port.Adapter.Rest.Resources
{
    /// <summary>
    /// Rest service for serving requests related to Trades
    /// </summary>
    [RoutePrefix("v1")]
    public class TradeController : ApiController
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger
     (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private ITradeApplicationService _tradeApplicationService;
        private IApiKeyInfoAccess _apiKeyInfoAccess;

        public TradeController(ITradeApplicationService tradeApplicationService,IApiKeyInfoAccess apiKeyInfoAccess)
        {
            _tradeApplicationService = tradeApplicationService;
            _apiKeyInfoAccess = apiKeyInfoAccess;
        }

        /// <summary>
        /// Private call that returns orders of the user that have been filled/executed
        /// TradeHistoryParams.Offset: Result offset (Optional)
        /// TradeHistoryParamsType.Type: Type of trade (optional) [all = all types (default), any position = any position (open or closed), closed position = positions that have been closed, closing position = any trade closing all or part of a position, no position = non-positional trades]</param>
        /// TradeHistoryParamsType.Trades: Whether or not to include trades related to position in output (optional.  default = false)
        /// TradeHistoryParamsType.Start: Starting unix timestamp or trade tx id of results (optional.  exclusive)
        /// TradeHistoryParamsType.End: Ending unix timestamp or trade tx id of results (optional.  inclusive)
        /// </summary>
        /// <returns></returns>
        [Route("trades/tradehistory")]
        [Authorize]
        [HttpPost]
        [ResponseType(typeof(IList<object>))]
        public IHttpActionResult GetTradeHistory([FromBody]TradeHistoryParams tradeHistoryParams)
        {
            if (log.IsDebugEnabled)
            {
                log.Debug("Trade History Call Recevied:" + tradeHistoryParams);
            }
            try
            {
                //get api key from header
                var headers = Request.Headers;
                string apikey = "";
                IEnumerable<string> headerParams;
                if (headers.TryGetValues("Auth", out headerParams))
                {
                    string[] auth = headerParams.ToList()[0].Split(',');
                    apikey = auth[0];
                }
                log.Debug("Api Key:" + apikey);
                TraderId traderId = new TraderId(_apiKeyInfoAccess.GetUserIdFromApiKey(apikey).ToString());
                var closedOrders = _tradeApplicationService.GetTradesHistory(traderId,tradeHistoryParams.Start, tradeHistoryParams.End);

                if (closedOrders != null)
                {
                    return Ok(closedOrders);
                }
                return BadRequest();
            }
            catch (Exception exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("Trade History Call Error", exception);
                }
                return InternalServerError();
            }
        }

        /// <summary>
        /// Private call that returns orders of the user that have been filled/executed
        /// QueryTradeParams.TxId: Comma separated list of txIds(Optional)
        /// QueryTradeParams.IncludeTrades: Whether or not to include the trades(Optional)
        /// </summary>
        /// <returns></returns>
        [Route("trades/querytrades")]
        [Authorize]
        [HttpPost]
        [ResponseType(typeof(IList<object>))]
        public IHttpActionResult QueryTrades([FromBody]string orderId)
        {
            try
            {
                //get api key from header
                var headers = Request.Headers;
                string apikey = "";
                IEnumerable<string> headerParams;
                if (headers.TryGetValues("Auth", out headerParams))
                {
                    string[] auth = headerParams.ToList()[0].Split(',');
                    apikey = auth[0];
                }
                AssertionConcern.AssertNullOrEmptyString(orderId, "OrderId cannot be null or empty");
                var trades = _tradeApplicationService.QueryTrades(orderId);

                if (trades != null)
                {
                    return Ok(trades);
                }
                return BadRequest();
            }
            catch (ArgumentNullException exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("Query Trades Call Error", exception);
                }
                return BadRequest(exception.Message);
            }
            catch (Exception exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("Query Trades Call Error", exception);
                }
                return InternalServerError();
            }
        }

        /// <summary>
        /// Private call that returns orders of the user that have been filled/executed
        /// QueryTradeParams.TxId: Comma separated list of txIds(Optional)
        /// QueryTradeParams.IncludeTrades: Whether or not to include the trades(Optional)
        /// </summary>
        /// <returns></returns>
        [Route("trades/tradedetails")]
        [Authorize]
        [HttpPost]
        [ResponseType(typeof(TradeDetailsRepresentation))]
        public IHttpActionResult TradeDetails([FromBody]string tradeId)
        {
            try
            {
                //get api key from header
                var headers = Request.Headers;
                string apikey = "";
                IEnumerable<string> headerParams;
                if (headers.TryGetValues("Auth", out headerParams))
                {
                    string[] auth = headerParams.ToList()[0].Split(',');
                    apikey = auth[0];
                }
                AssertionConcern.AssertNullOrEmptyString(tradeId, "TradeId cannot be null or empty");
                TraderId traderId = new TraderId(_apiKeyInfoAccess.GetUserIdFromApiKey(apikey).ToString());
                var trades = _tradeApplicationService.GetTradeDetails(traderId.Id, tradeId);

                if (trades != null)
                {
                    return Ok(trades);
                }
                return BadRequest();
            }
            catch (ArgumentNullException exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("Trade Details Call Error", exception);
                }
                return BadRequest(exception.Message);
            }
            catch (Exception exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("Trade Details Call Error", exception);
                }
                return InternalServerError();
            }
        }

        /// <summary>
        /// Public call to get recent trades
        /// </summary>
        /// <param name="currencyPair"> </param>
        /// <param name="since"> </param>
        /// <returns></returns>
        [Route("trades/recenttrades")]
        [HttpGet]
        [ResponseType(typeof(IList<object>))]
        public IHttpActionResult RecentTrades(string currencyPair, string since = "")
        {
            if (log.IsDebugEnabled)
            {
                log.Debug(string.Format("Recent Trades call: currency pair={0}",currencyPair));
            }
            try
            {
                AssertionConcern.AssertNullOrEmptyString(currencyPair, "CurrencyPair cannot be null or empty");
                var trades = _tradeApplicationService.GetRecentTrades(currencyPair, since);

                if (trades != null)
                {
                    return Ok(trades);
                }
                return BadRequest();
            }
            catch (ArgumentNullException exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("Recent Trades Call Error", exception);
                }
                return BadRequest(exception.Message);
            }
            catch (Exception exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("Recent Trades Call Error", exception);
                }
                return InternalServerError(exception);
            }
        }

        /// <summary>
        /// private call to request trade volume
        /// </summary>
        /// <param name="pair"> </param>
        /// <returns></returns>
        [Route("trades/tradevolume")]
        [Authorize]
        [HttpPost]
        [ResponseType(typeof(TradeVolumeRepresentation))]
        public IHttpActionResult TradeVolume([FromBody]string pair)
        {
            try
            {
                if (pair != string.Empty)
                {
                    return Ok(_tradeApplicationService.TradeVolume(pair));
                }
                return BadRequest();
            }
            catch (Exception ex)
            {
                return InternalServerError();
            }
        }

        /// <summary>
        /// List of tradeable currency pairs
        /// </summary>
        /// <returns></returns>
        [Route("trades/tradeablecurrencypair")]
        [HttpGet]
        [ResponseType(typeof(IList<CurrencyPair>))]
        public IHttpActionResult TradeableCurrencyPair()
        {
            if (log.IsDebugEnabled)
            {
                log.Debug("Tradeabale Currency Pair Call");
            }
            try
            {
                return Ok(_tradeApplicationService.GetTradeableCurrencyPairs());
            }
            catch (Exception exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("Recent Trades Call Error", exception);
                }
                return InternalServerError();
            }
        }
  
    }
}

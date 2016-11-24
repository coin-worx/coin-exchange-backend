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
using CoinExchange.Trades.ReadModel.DTO;

namespace CoinExchange.Trades.ReadModel.Repositories
{
    public interface ITradeRepository
    {
        /// <summary>
        /// Public call for getting recent trades
        /// </summary>
        /// <param name="lastId"></param>
        /// <param name="pair"></param>
        /// <returns></returns>
        IList<object> GetRecentTrades(string lastId,string pair);

        /// <summary>
        /// To get traders trade history
        /// </summary>
        /// <param name="traderId"></param>
        /// <returns></returns>
        IList<object> GetTraderTradeHistory(string traderId);

        /// <summary>
        /// Get trades between specified date
        /// </summary>
        /// <param name="traderId"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        IList<object> GetTraderTradeHistory(string traderId, DateTime start, DateTime end);

        /// <summary>
        /// Get by Trade Id
        /// </summary>
        /// <param name="tradeId"></param>
        /// <returns></returns>
        TradeReadModel GetById(string tradeId);

        /// <summary>
        /// Get by Trade Id and traderId
        /// </summary>
        /// <param name="tradeId"></param>
        /// <returns></returns>
        TradeReadModel GetByIdAndTraderId(string traderId,string tradeId);

        /// <summary>
        /// Get trades between dates
        /// </summary>
        /// <param name="end"></param>
        /// <param name="start"></param>
        /// <param name="currencyPair"></param>
        /// <returns></returns>
        IList<TradeReadModel> GetTradesBetweenDates(DateTime end, DateTime start,string currencyPair);

        /// <summary>
        /// Get All trades
        /// </summary>
        /// <returns></returns>
        IList<TradeReadModel> GetAll();

        /// <summary>
        /// Get custom calculated data for ticker info calculation
        /// </summary>
        /// <param name="end"></param>
        /// <param name="start"></param>
        /// <param name="currencyPair"></param>
        /// <returns></returns>
        object GetCustomDataBetweenDates(DateTime end, DateTime start,string currencyPair);

        void RollBack();

        /// <summary>
        /// Get Trades By OrderID
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        IList<object> GetTradesByorderId(string orderId);
    }
}

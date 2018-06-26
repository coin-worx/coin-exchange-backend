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
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Trades.Domain.Model.TradeAggregate;

namespace CoinExchange.Trades.ReadModel.DTO
{
    public class OhlcReadModel
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger
     (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public OhlcReadModel()
        {
            
        }

        public OhlcReadModel(string currencyPair,DateTime dateTime, decimal open, decimal high, decimal low, decimal close, decimal volume)
        {
            CurrencyPair = currencyPair;
            DateTime = dateTime;
            Open = open;
            High = high;
            Low = low;
            Close = close;
            Volume = volume;
            TotalWeight = open*volume;
            AveragePrice = TotalWeight/volume;
        }

        /// <summary>
        /// Update ohlc to recent trade
        /// </summary>
        /// <param name="latestTrade"></param>
        public void UpdateOhlc(Trade latestTrade)
        {
            //update high
            if (latestTrade.ExecutionPrice.Value > High)
                High = latestTrade.ExecutionPrice.Value;
            //update low
            if (latestTrade.ExecutionPrice.Value < Low)
                Low = latestTrade.ExecutionPrice.Value;
            //assign new calculated volume
            Volume += latestTrade.ExecutedVolume.Value;
            //assign new close
            Close = latestTrade.ExecutionPrice.Value;
            //calculate weighted average price
            TotalWeight += latestTrade.ExecutionPrice.Value*latestTrade.ExecutedVolume.Value;
            AveragePrice = TotalWeight/Volume;
        }

        public int Id { get; set; }
        public DateTime DateTime { get; set; }
        public decimal Open { get; set; }
        public decimal High { get; set; }
        public decimal Low { get; set; }
        public decimal Close { get; set; }
        public decimal Volume { get; set; }
        public string CurrencyPair { get; set; }
        public decimal TotalWeight { get; set; }
        public decimal AveragePrice { get; set; }
    }
}

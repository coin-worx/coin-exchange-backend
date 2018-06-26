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
using CoinExchange.Trades.ReadModel.MemoryImages;

namespace CoinExchange.Trades.ReadModel.DTO
{
    /// <summary>
    /// Currency pairs ticker information
    /// </summary>
    public class TickerInfoReadModel
    {
        public int Id { get; set; }
        public decimal AskPrice { get; set; }
        public decimal AskVolume { get; set; }
        public decimal BidPrice { get; set; }
        public decimal BidVolume { get; set; }
        public decimal TradePrice { get; set; }
        public decimal TradeVolume { get; set; }
        public decimal TodaysVolume { get; set; }
        public decimal Last24HourVolume { get; set; }
        public decimal TodaysVolumeWeight { get; set; }
        public decimal Last24HourVolumeWeight { get; set; }
        public long TodaysTrades { get; set; }
        public long Last24HourTrades { get; set; }
        public decimal TodaysLow { get; set; }
        public decimal Last24HoursLow { get; set; }
        public decimal TodaysHigh { get; set; }
        public decimal Last24HoursHigh { get; set; }
        public decimal OpeningPrice { get; set; }
        public string CurrencyPair { get; set; }

        public TickerInfoReadModel()
        {
            
        }

        public TickerInfoReadModel(string currencyPair, decimal price, decimal volume)
        {
            CurrencyPair = currencyPair;
            OpeningPrice = TodaysHigh = TodaysLow = Last24HoursHigh = Last24HoursLow = TradePrice = price;
            TradeVolume = TodaysVolume = Last24HourVolume = TodaysVolumeWeight = Last24HourVolumeWeight = volume;
            TodaysTrades = Last24HourTrades = 1;
        }

        /// <summary>
        /// Update ticker info from trades
        /// </summary>
        /// <param name="today"></param>
        /// <param name="last24Hour"></param>
        /// <param name="openingPrice"></param>
        /// <param name="lastTradePrice"></param>
        /// <param name="lastTradeVolume"></param>
        public void UpdateTickerInfo(object[] today, object[] last24Hour,decimal openingPrice,decimal lastTradePrice,decimal lastTradeVolume)
        {
            //update only if more than one records are found
            if ((long) today[0] >= 1)
            {
                TodaysTrades = (long) today[0];
                //Last24HourTrades = (long) last24Hour[0];
                TodaysHigh = (decimal) today[1];
                //Last24HoursHigh = (decimal) last24Hour[1];
                TodaysLow = (decimal) today[2];
                //Last24HoursLow = (decimal) last24Hour[2];
                TodaysVolume = (decimal) today[3];
                //Last24HourVolume = (decimal) last24Hour[3];
                TodaysVolumeWeight = (decimal) today[4];
                //Last24HourVolumeWeight = (decimal) last24Hour[4];
            }
            if ((long)last24Hour[0] >= 1)
            {
                //TodaysTrades = (long)today[0];
                Last24HourTrades = (long)last24Hour[0];
                //TodaysHigh = (decimal)today[1];
                Last24HoursHigh = (decimal)last24Hour[1];
                //TodaysLow = (decimal)today[2];
                Last24HoursLow = (decimal)last24Hour[2];
                //TodaysVolume = (decimal)today[3];
                Last24HourVolume = (decimal)last24Hour[3];
                //TodaysVolumeWeight = (decimal)today[4];
                Last24HourVolumeWeight = (decimal)last24Hour[4];
            }
            TradePrice = lastTradePrice;
            TradeVolume = lastTradeVolume;
            OpeningPrice = openingPrice;
        }

        /// <summary>
        /// Append bbo info to ticker info
        /// </summary>
        /// <param name="bboRepresentation"></param>
        public void UpdateBboInTickerInfo(BBORepresentation bboRepresentation)
        {
            AskPrice = bboRepresentation.BestAskPrice;
            AskVolume = bboRepresentation.BestAskVolume;
            BidPrice = bboRepresentation.BestBidPrice;
            BidVolume = bboRepresentation.BestBidVolume;
        }
    }
}

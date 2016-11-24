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
using CoinExchange.Trades.ReadModel.DTO;
using CoinExchange.Trades.ReadModel.Repositories;

namespace CoinExchange.Trades.ReadModel.Services
{
    /// <summary>
    /// Service for calculating ticker information
    /// </summary>
    public class TickerInfoCalculation
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger
     (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private IPersistanceRepository _persistanceRepository;
        private ITradeRepository _tradeRepository;
        private ITickerInfoRepository _tickerInfoRepository;

        public TickerInfoCalculation(ITickerInfoRepository tickerInfoRepository, ITradeRepository tradeRepository, IPersistanceRepository persistanceRepository)
        {
            _tickerInfoRepository = tickerInfoRepository;
            _tradeRepository = tradeRepository;
            _persistanceRepository = persistanceRepository;
        }

        /// <summary>
        /// Calculate ticker info from recent trade and update it
        /// </summary>
        /// <param name="trade"></param>
        public void CalculateTickerInfo(Trade trade)
        {
            var tickerReadModel = _tickerInfoRepository.GetTickerInfoByCurrencyPair(trade.CurrencyPair);
            if (tickerReadModel == null)
            {
                TickerInfoReadModel model=new TickerInfoReadModel(trade.CurrencyPair,trade.ExecutionPrice.Value,trade.ExecutedVolume.Value);
                //model.CurrencyPair = trade.CurrencyPair;
                _persistanceRepository.SaveOrUpdate(model);
            }
            else
            {
                object[] todays=CalculateTodaysData(trade);
                log.Debug("Recevied today:"+todays);
                object[] last24Hours=Calculate24HoursData(trade);
                log.Debug("Received 24Hours:"+last24Hours);
                decimal openingPrice=0;
                decimal lastTradePrice=0;
                decimal lastTradeVolume=0;
                IList<TradeReadModel> trades = _tradeRepository.GetTradesBetweenDates(trade.ExecutionTime, DateTime.Today,trade.CurrencyPair);
                if (trades != null)
                {
                    if (trades.Count > 0)
                    {
                        log.Debug("Total Trades=" + trades.Count);
                        openingPrice = trades[trades.Count - 1].Price;
                        lastTradePrice = trades[0].Price;
                        lastTradeVolume = trades[0].Volume;
                    }
                }
                tickerReadModel.UpdateTickerInfo(todays,last24Hours,openingPrice,lastTradePrice,lastTradeVolume);
                _persistanceRepository.SaveOrUpdate(tickerReadModel);
            }
        }

        /// <summary>
        /// Calculation fo data from todays 00:00:00
        /// </summary>
        private object[] CalculateTodaysData(Trade trade)
        {
            log.Debug("Start Date:"+DateTime.Today+", End Date="+DateTime.Now);
            return _tradeRepository.GetCustomDataBetweenDates(trade.ExecutionTime, DateTime.Today,trade.CurrencyPair) as object[];
        }

        /// <summary>
        /// Calculations for 24 hours of data
        /// </summary>
        private object[] Calculate24HoursData(Trade trade)
        {
            log.Debug("Start Date:" + DateTime.Now.AddDays(-1) + ", End Date=" + DateTime.Now);
            return _tradeRepository.GetCustomDataBetweenDates(trade.ExecutionTime, trade.ExecutionTime.AddDays(-1),trade.CurrencyPair) as object[];
        }
    }
}

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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Trades.Domain.Model.MarketDataAggregate;
using CoinExchange.Trades.Domain.Model.TradeAggregate;
using CoinExchange.Trades.ReadModel.DTO;
using CoinExchange.Trades.ReadModel.Repositories;

namespace CoinExchange.Trades.ReadModel.Services
{
    /// <summary>
    /// Service for calculating one minute Ohlc on new trade arrival
    /// </summary>
    public class OhlcCalculation
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger
        (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private IPersistanceRepository _persistanceRepository;
        private ITradeRepository _tradeRepository;
        private IOhlcRepository _ohlcRepository;
        
        public OhlcCalculation(IPersistanceRepository persistanceRepository,IOhlcRepository ohlcRepository,ITradeRepository tradeRepository)
        {
            _persistanceRepository = persistanceRepository;
            _ohlcRepository = ohlcRepository;
            _tradeRepository = tradeRepository;
        }

        public void CalculateAndPersistOhlc(Trade latestTrade)
        {
            DateTime ohlcdDateTime = latestTrade.ExecutionTime.AddSeconds(-1*latestTrade.ExecutionTime.Second);
            //IList<TradeReadModel> trades=_tradeRepository.GetTradesBetweenDates(latestTrade.ExecutionTime,ohlcdDateTime);
            OhlcReadModel model = _ohlcRepository.GetOhlcByDateTime(ohlcdDateTime.AddMinutes(1));
            if (model == null)
            {
                //means it is 1st trade of that minute
                OhlcReadModel newOhlcReadModel = new OhlcReadModel(latestTrade.CurrencyPair,ohlcdDateTime.AddMinutes(1), latestTrade.ExecutionPrice.Value,
                    latestTrade.ExecutionPrice.Value,
                    latestTrade.ExecutionPrice.Value, latestTrade.ExecutionPrice.Value, latestTrade.ExecutedVolume.Value);
                _persistanceRepository.SaveOrUpdate(newOhlcReadModel);
            }
            else
            {
               // IList<TradeReadModel> trades = _tradeRepository.GetTradesBetweenDates(latestTrade.ExecutionTime,
                 //   ohlcdDateTime);
                //decimal volume = CalculateVolume(trades);
                //update the ohlc
                model.UpdateOhlc(latestTrade);
                _persistanceRepository.SaveOrUpdate(model);
            }

        }

        /// <summary>
        /// Calculate average volume of trades
        /// </summary>
        /// <param name="trades"></param>
        /// <returns></returns>
        private decimal CalculateVolume(IList<TradeReadModel> trades)
        {
            decimal volume = 0;
            foreach (var tradeReadModel in trades)
            {
                volume += tradeReadModel.Volume;
            }
            volume = volume/trades.Count;
            return volume;
        }
        
    }
}

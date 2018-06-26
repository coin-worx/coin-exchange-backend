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
using CoinExchange.Trades.Application.OrderServices.Representation;
using CoinExchange.Trades.Application.TradeServices.Representation;
using CoinExchange.Trades.Domain.Model.CurrencyPairAggregate;
using CoinExchange.Trades.Domain.Model.OrderAggregate;
using CoinExchange.Trades.Domain.Model.TradeAggregate;
using CoinExchange.Trades.ReadModel.DTO;
using CoinExchange.Trades.ReadModel.Repositories;

namespace CoinExchange.Trades.Application.TradeServices
{
    public class TradeApplicationService : ITradeApplicationService
    {
        private ITradeRepository _tradeRepository;
        private ICurrencyPairRepository _currencyPairRepository;
        private IOrderRepository _orderRepository;

        public TradeApplicationService(ITradeRepository tradeRepository,ICurrencyPairRepository currencyPairRepository,IOrderRepository orderRepository)
        {
            _tradeRepository = tradeRepository;
            _currencyPairRepository = currencyPairRepository;
            _orderRepository = orderRepository;
        }

        public object GetTradesHistory(TraderId traderId, string start = "", string end = "")
        {
            if (start == "" || end == "")
            {
                return _tradeRepository.GetTraderTradeHistory(traderId.Id.ToString());
            }
            return _tradeRepository.GetTraderTradeHistory(traderId.Id.ToString(), Convert.ToDateTime(start),
                Convert.ToDateTime(end));
        }

        public object QueryTrades(string orderId)
        {
            return _tradeRepository.GetTradesByorderId(orderId);
        }

        public IList<object> GetRecentTrades(string pair, string since)
        {
            return _tradeRepository.GetRecentTrades(since, pair);
        }

        public TradeVolumeRepresentation TradeVolume(string pair)
        {
            throw new NotImplementedException();
        }


        public IList<CurrencyPair> GetTradeableCurrencyPairs()
        {
            return _currencyPairRepository.GetTradeableCurrencyPairs();
        }

        /// <summary>
        /// Trade Details
        /// </summary>
        /// <param name="traderId"></param>
        /// <param name="tradeId"></param>
        /// <returns></returns>
        public TradeDetailsRepresentation GetTradeDetails(string traderId, string tradeId)
        {
            TradeReadModel model = _tradeRepository.GetByIdAndTraderId(traderId,tradeId);
            OrderReadModel buyOrder = _orderRepository.GetOrderById(model.BuyOrderId);
            OrderReadModel sellOrder = _orderRepository.GetOrderById(model.SellOrderId);
            if (buyOrder.TraderId == traderId && sellOrder.TraderId == traderId)
            {
                OrderReadModel fillingOrder = buyOrder.DateTime > sellOrder.DateTime ? buyOrder : sellOrder;
                return new TradeDetailsRepresentation(fillingOrder, model.ExecutionDateTime, model.Price, model.Volume, model.TradeId);
            }
            if (buyOrder.TraderId == traderId)
            {
                return new TradeDetailsRepresentation(buyOrder,model.ExecutionDateTime,model.Price,model.Volume,model.TradeId);
            }
            return new TradeDetailsRepresentation(sellOrder,model.ExecutionDateTime,model.Price,model.Volume,model.TradeId);
        }
    }
}

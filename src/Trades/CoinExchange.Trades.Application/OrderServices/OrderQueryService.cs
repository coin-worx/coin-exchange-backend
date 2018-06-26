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
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Trades.Application.OrderServices.Representation;
using CoinExchange.Trades.Domain.Model.OrderAggregate;
using CoinExchange.Trades.Domain.Model.TradeAggregate;
using CoinExchange.Trades.ReadModel.DTO;
using CoinExchange.Trades.ReadModel.Repositories;

namespace CoinExchange.Trades.Application.OrderServices
{
    /// <summary>
    /// Queries information for the orders
    /// </summary>
    public class OrderQueryService : IOrderQueryService
    {
        private IOrderRepository _orderRepository = null;
        private ITradeRepository _tradeRepository;

        /// <summary>
        /// Default Constructor
        /// </summary>
        public OrderQueryService(IOrderRepository orderRepository,ITradeRepository tradeRepository)
        {
            _orderRepository = orderRepository;
            _tradeRepository = tradeRepository;
        }

        #region Implementation of IOrderQueryService

        /// <summary>
        /// Gets a List of open orders represented as ReadModel
        /// </summary>
        /// <param name="traderId"></param>
        /// <param name="includeTrades"></param>
        /// <returns></returns>
        public object GetOpenOrders(TraderId traderId, bool includeTrades = false)
        {
            List<OrderReadModel> orders = _orderRepository.GetOpenOrders(traderId.Id.ToString(CultureInfo.InvariantCulture));
            for (int i = 0; i < orders.Count; i++)
            {
                IList<object> trades = _tradeRepository.GetTradesByorderId(orders[i].OrderId);
                Tuple<decimal, DateTime?> data = CalculateAveragePrice(trades);
                orders[i].AveragePrice = data.Item1;
                if (orders[i].Status == OrderState.Complete.ToString())
                {
                    orders[i].ClosingDateTime = data.Item2;
                }
                if (includeTrades)
                {
                    orders[i].Trades = trades;
                }
            }
            return orders;
        }

        /// <summary>
        /// Gets a List of closed orders represented as ReadModel
        /// </summary>
        /// <param name="traderId"></param>
        /// <param name="includeTrades"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public object GetClosedOrders(TraderId traderId, bool includeTrades = false, string startTime = "", string endTime = "")
        {
            List<OrderReadModel> orders;
            if (startTime == "" || endTime == "")
            {
                orders =_orderRepository.GetClosedOrders(traderId.Id.ToString(CultureInfo.InvariantCulture));
            }
            else
            {
                orders = _orderRepository.GetClosedOrders(traderId.Id.ToString(), Convert.ToDateTime(startTime),
                    Convert.ToDateTime(endTime));
            }
            for (int i = 0; i < orders.Count; i++)
            {
                IList<object> trades = _tradeRepository.GetTradesByorderId(orders[i].OrderId);
                Tuple<decimal, DateTime?> data = CalculateAveragePrice(trades);
                orders[i].AveragePrice = data.Item1;
                if (orders[i].Status == OrderState.Complete.ToString())
                {
                    orders[i].ClosingDateTime = data.Item2;
                }
                if (includeTrades)
                {
                    orders[i].Trades = trades;
                }
            }
            return orders;
        }

        #endregion
        
        public object GetOrderById(TraderId traderId,OrderId orderId)
        {
            OrderReadModel order = _orderRepository.GetOrderById(traderId, orderId);
            Tuple<decimal, DateTime?> data = CalculateAveragePrice(_tradeRepository.GetTradesByorderId(orderId.Id));
            order.AveragePrice = data.Item1;
            if (order.Status == OrderState.Complete.ToString())
            {
                order.ClosingDateTime = data.Item2;
            }
            return order;
        }

        /// <summary>
        /// Calculate avergae price from trades.
        /// </summary>
        /// <returns></returns>
        private Tuple<decimal,DateTime?> CalculateAveragePrice(IList<object> trades)
        {
            decimal price = 0;
            DateTime? closingDateTime=null;
            if (trades != null)
            {
                if (trades.Count > 0)
                {
                    for (int i = 0; i < trades.Count; i++)
                    {
                        object[] trade = trades[i] as object[];
                        price = price + (decimal) trade[2];
                        if (i == 0)
                        {
                            closingDateTime = (DateTime) trade[1];
                        }
                        else
                        {
                            if ((DateTime) trade[1] >= closingDateTime)
                            {
                                closingDateTime = (DateTime) trade[1];
                            }
                        }
                    }
                    price = price/trades.Count;
                }
            }
            return new Tuple<decimal, DateTime?>(price,closingDateTime);
        }
    }
}

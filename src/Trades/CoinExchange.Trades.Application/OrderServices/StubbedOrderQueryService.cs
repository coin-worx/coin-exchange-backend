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
using CoinExchange.Trades.Application.OrderServices.Representation;
using CoinExchange.Trades.Domain.Model.OrderAggregate;
using CoinExchange.Trades.Domain.Model.TradeAggregate;
using CoinExchange.Trades.ReadModel.DTO;

namespace CoinExchange.Trades.Application.OrderServices
{
    /// <summary>
    /// Handles the query requests related to Orders
    /// </summary>
    public class StubbedOrderQueryService : IOrderQueryService
    {
        /// <summary>
        /// Gets the list for the Open orders
        /// </summary>
        /// <returns></returns>
        public object GetOpenOrders(TraderId traderId, bool includeTrades = false)
        {
            List<OrderRepresentation> orderList = new List<OrderRepresentation>();
            orderList.Add(new OrderRepresentation()
            {
                TxId = "EEER342",
                UserRefId = "WREDF342",
                Pair = "XBTUSD",
                Status = OrderState.Accepted,
                OpenTime = DateTime.Now.AddHours(-2),
                ExpireTime = DateTime.Now.AddHours(3),
                Volume = 3000,
                Cost = 0,
                Fee = (decimal?)0.25,
                Price = (decimal)491.23,
                StopPrice = 0,
                OFlags = "DUMMY",
                Trades = "23453,1764,1554,2134",
                Opened = "045643.23"
            });
            orderList.Add(new OrderRepresentation()
            {
                TxId = "EEER342",
                UserRefId = "YIO468S",
                Pair = "XBTEURS",
                Status = OrderState.Accepted,
                OpenTime = DateTime.Now.AddHours(-2),
                ExpireTime = DateTime.Now.AddHours(3),
                Volume = 3000,
                Cost = 0,
                Fee = (decimal?)0.25,
                Price = (decimal)491.23,
                StopPrice = 0,
                OFlags = "DUMMY",
                Trades = "23453,1764,1554,2134",
                Opened = "045643.23"
            });
            orderList.Add(new OrderRepresentation()
            {
                TxId = "EEER342",
                UserRefId = "GTII5769",
                Pair = "LTCUSD",
                Status = OrderState.Accepted,
                OpenTime = DateTime.Now.AddHours(-2),
                ExpireTime = DateTime.Now.AddHours(3),
                Volume = 3000,
                Cost = 0,
                Fee = (decimal?)0.25,
                Price = (decimal)491.23,
                StopPrice = 0,
                OFlags = "DUMMY",
                Trades = "23453,1764,1554,2134",
                Opened = "045643.23"
            });

            return orderList;
        }

        /// <summary>
        /// Gets the list for the Closed orders
        /// </summary>
        /// <returns></returns>
        public object GetClosedOrders(TraderId traderId, bool includeTrades = false,string startTime = "", string endTime = "")
        {
            List<OrderRepresentation> orderList = new List<OrderRepresentation>();
            orderList.Add(new OrderRepresentation()
            {
                TxId = "EEER342",
                UserRefId = "WREDF342",
                Pair = "XBTUSD",
                Status = OrderState.Cancelled,
                OpenTime = DateTime.Now.AddHours(-2),
                ExpireTime = DateTime.Now.AddHours(3),
                Volume = 3000,
                Cost = 0,
                Fee = (decimal?)0.25,
                Price = (decimal)491.23,
                StopPrice = 0,
                OFlags = "DUMMY",
                Trades = "23453,1764,1554,2134",
                Reason = "Insufficient Volume",
                Closed = "045643.23"
            });
            orderList.Add(new OrderRepresentation()
            {
                TxId = "EEER342",
                UserRefId = "YIO468S",
                Pair = "XBTEURS",
                Status = OrderState.Expired,
                OpenTime = DateTime.Now.AddHours(-2),
                ExpireTime = DateTime.Now.AddHours(3),
                Volume = 3000,
                Cost = 0,
                Fee = (decimal?)0.25,
                Price = (decimal)491.23,
                StopPrice = 0,
                OFlags = "DUMMY",
                Trades = "23453,1764,1554,2134",
                Reason = "Insufficient Volume",
                Closed = "045643.23"
            });
            orderList.Add(new OrderRepresentation()
            {
                TxId = "EEER342",
                UserRefId = "GTII5769",
                Pair = "LTCUSD",
                Status = OrderState.Accepted,
                OpenTime = DateTime.Now.AddHours(-2),
                ExpireTime = DateTime.Now.AddHours(3),
                Volume = 3000,
                Cost = 0,
                Fee = (decimal?)0.25,
                Price = (decimal)491.23,
                StopPrice = 0,
                OFlags = "DUMMY",
                Trades = "23453,1764,1554,2134",
                Reason = "Insufficient Volume",
                Closed = "045643.23"
            });

            return orderList;
        }


        public object GetOrderById(TraderId traderId,OrderId orderId)
        {
            return new OrderReadModel(orderId.Id, "market", "buy", 0, 10, traderId.Id, "Closed", "XBTUSD", DateTime.Now, 10, 0,DateTime.Now);
        }
    }
}

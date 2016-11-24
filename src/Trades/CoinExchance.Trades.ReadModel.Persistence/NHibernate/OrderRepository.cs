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
using CoinExchange.Trades.Domain.Model.OrderAggregate;
using CoinExchange.Trades.Domain.Model.TradeAggregate;
using CoinExchange.Trades.ReadModel.DTO;
using CoinExchange.Trades.ReadModel.Repositories;
using NHibernate.Criterion;
using NHibernate.Linq;
using Spring.Stereotype;
using Spring.Transaction.Interceptor;

namespace CoinExchange.Trades.ReadModel.Persistence.NHibernate
{
    /// <summary>
    /// Order repository implementation
    /// </summary>
    [Repository]
    public class OrderRepository : NHibernateSessionFactory,IOrderRepository
    {
        [Transaction(ReadOnly = true)]
        public List<OrderReadModel> GetOpenOrders(string traderId)
        {
            return CurrentSession.Query<OrderReadModel>()
                .Where(order => order.TraderId.Equals(traderId) && 
                (order.Status.Equals("New") || order.Status.Equals("Accepted") || order.Status.Equals("PartiallyFilled")))
                .AsQueryable()
                .OrderByDescending(x => x.DateTime)
                .ToList();
        }

        [Transaction(ReadOnly = true)]
        public List<OrderReadModel> GetClosedOrders(string traderId,DateTime start,DateTime end)
        {
            return CurrentSession.Query<OrderReadModel>()
                .Where(order => order.TraderId.Equals(traderId) &&
                      (order.Status.Equals("Cancelled") || order.Status.Equals("Rejected") || order.Status.Equals("Complete"))
                      && order.DateTime >= start && order.DateTime <= end)
                .AsQueryable()
                .OrderBy(x => x.DateTime)
                .ToList();
        }

        [Transaction(ReadOnly = true)]
        public List<OrderReadModel> GetClosedOrders(string traderId)
        {
            return CurrentSession.Query<OrderReadModel>()
                .Where(order => order.TraderId.Equals(traderId) &&
                (order.Status.Equals("Cancelled") || order.Status.Equals("Rejected") || order.Status.Equals("Complete")))
                .AsQueryable()
                .OrderByDescending(x => x.DateTime)
                .ToList();
        }

        [Transaction(ReadOnly = true)]
        public List<OrderReadModel> GetAllOrderOfTrader(string traderId)
        {
            return CurrentSession.Query<OrderReadModel>()
                .Where(order => order.TraderId.Equals(traderId))
                .AsQueryable()
                .ToList();
        }

        [Transaction(ReadOnly = true)]
        public OrderReadModel GetOrderById(string orderId)
        {
            return CurrentSession.Get<OrderReadModel>(orderId);
        }

        [Transaction(ReadOnly = true)]
        public OrderReadModel GetOrderById(TraderId traderId,OrderId orderId)
        {
            OrderReadModel model=CurrentSession.Get<OrderReadModel>(orderId.Id);
            if (model.TraderId.Equals(traderId.Id, StringComparison.InvariantCultureIgnoreCase))
                return model;
            return null;
        }

        [Transaction(ReadOnly = false)]
        public void RollBack()
        {
            string sqlQuery = string.Format("DELETE FROM COINEXCHANGE.ORDER");

            CurrentSession.CreateSQLQuery(sqlQuery).ExecuteUpdate();
        }
    }
}

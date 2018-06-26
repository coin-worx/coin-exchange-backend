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
using System.Security.Cryptography.X509Certificates;
using CoinExchange.Trades.ReadModel.DTO;
using CoinExchange.Trades.ReadModel.Repositories;
using NHibernate.Linq;
using NHibernate.Type;
using Spring.Stereotype;
using Spring.Transaction.Interceptor;

namespace CoinExchange.Trades.ReadModel.Persistence.NHibernate
{
    [Repository]
    public class TradeRepository : NHibernateSessionFactory,ITradeRepository
    {
        [Transaction(ReadOnly = true)]
        public IList<object> GetRecentTrades(string lastId, string pair)
        {
            return CurrentSession.QueryOver<TradeReadModel>().Select(t => t.ExecutionDateTime, t => t.Price, t => t.Volume).OrderBy(x=>x.ExecutionDateTime).Desc.List<object>();
        }

        //[Transaction(ReadOnly = true)]
        //public IList<TradeReadModel> GetTraderTradeHistory(string traderId)
        //{
        //    return CurrentSession.Query<TradeReadModel>()
        //        .Where(trade => trade.BuyTraderId.Equals(traderId)||trade.SellTraderId.Equals(traderId))
        //        .AsQueryable()
        //        .ToList();
        //}

        [Transaction(ReadOnly = true)]
        public IList<object> GetTraderTradeHistory(string traderId)
        {
            return
                CurrentSession.QueryOver<TradeReadModel>().Select(t => t.TradeId, t => t.ExecutionDateTime, t => t.Price,
                                                                  t => t.Volume, t => t.CurrencyPair)
                    .Where(trade => trade.BuyTraderId == traderId || trade.SellTraderId == traderId)
                    .OrderBy(x => x.ExecutionDateTime)
                    .Desc
                    .List<object>();
        }

        [Transaction(ReadOnly = true)]
        public IList<object> GetTraderTradeHistory(string traderId,DateTime start,DateTime end)
        {
            return
                CurrentSession.QueryOver<TradeReadModel>().Select(t => t.TradeId, t => t.ExecutionDateTime, t => t.Price,
                                                                  t => t.Volume, t => t.CurrencyPair)
                    .Where(trade => trade.BuyTraderId == traderId || trade.SellTraderId == traderId)
                    .OrderBy(x => x.ExecutionDateTime)
                    .Asc
                    .List<object>();
        }

        [Transaction(ReadOnly = true)]
        public TradeReadModel GetById(string tradeId)
        {
            return CurrentSession.Get<TradeReadModel>(tradeId);
        }

        [Transaction(ReadOnly = true)]
        public IList<object> GetTradesByorderId(string orderId)
        {
            return CurrentSession.QueryOver<TradeReadModel>().Select(t => t.TradeId, t => t.ExecutionDateTime, t => t.Price, t => t.Volume, t => t.CurrencyPair)
                .Where(trade => trade.BuyOrderId == orderId || trade.SellOrderId == orderId)
                .List<object>();
        }

        [Transaction(ReadOnly = true)]
        public IList<TradeReadModel> GetTradesBetweenDates(DateTime end, DateTime start,string currencyPair)
        {
            return
                CurrentSession.QueryOver<TradeReadModel>()
                    .Where(x => x.ExecutionDateTime <= end && x.ExecutionDateTime >= start && x.CurrencyPair==currencyPair).OrderBy(x => x.ExecutionDateTime).Desc
                    .List();
        }

        [Transaction(ReadOnly = true)]
        public object GetCustomDataBetweenDates(DateTime end, DateTime start,string currencyPair)
        {
            string sqlQuery = string.Format("SELECT COUNT(TradeId) as NumberOfTrades,max(Price) as high, min(Price)as low, SUM(Volume)as Volume, SUM(Volume*Price)/SUM(Volume) as vwap FROM trade WHERE ExecutionDateTime >='{0}' AND ExecutionDateTime <='{1}' AND CurrencyPair='{2}'", start.ToString("u"), end.ToString("u"),currencyPair);
            var result = CurrentSession.CreateSQLQuery(sqlQuery).List();
            return result[0];
        }

        [Transaction(ReadOnly = false)]
        public void RollBack()
        {
            string sqlQuery = string.Format("DELETE FROM TRADE");

            CurrentSession.CreateSQLQuery(sqlQuery).ExecuteUpdate();
        }

        [Transaction(ReadOnly = true)]
        public TradeReadModel GetByIdAndTraderId(string traderId, string tradeId)
        {
            TradeReadModel model=CurrentSession.Get<TradeReadModel>(tradeId);
            if (model.BuyTraderId == traderId || model.SellTraderId == traderId)
            {
                return model;
            }
            throw new InvalidOperationException("Not Authorized");
        }

        [Transaction(ReadOnly = true)]
        public IList<TradeReadModel> GetAll()
        {
            return CurrentSession.QueryOver<TradeReadModel>().List<TradeReadModel>();
        }
    }
}

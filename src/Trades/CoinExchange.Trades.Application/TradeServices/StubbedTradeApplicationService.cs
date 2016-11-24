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
using CoinExchange.Trades.Application.TradeServices.Representation;
using CoinExchange.Trades.Domain.Model.CurrencyPairAggregate;
using CoinExchange.Trades.Domain.Model.OrderAggregate;
using CoinExchange.Trades.Domain.Model.TradeAggregate;

namespace CoinExchange.Trades.Application.TradeServices
{
    /// <summary>
    /// Service to serve operations related to Trades
    /// </summary>
    public class StubbedTradeApplicationService : ITradeApplicationService
    {
        /// <summary>
        /// Default Constructor
        /// </summary>
        public StubbedTradeApplicationService()
        {
            
        }

        /// <summary>
        /// Returns orders of the user that have been filled/executed
        /// </summary>
        /// <returns></returns>
        public object GetTradesHistory(TraderId traderId, string start = "", string end = "")
        {
            List<OrderRepresentation> orderList = new List<OrderRepresentation> ();
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
                Executed = "045643.23",
                VolumeExecuted = 2000
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
                Executed = "045643.23",
                VolumeExecuted = 2000
            });
            orderList.Add(new OrderRepresentation()
            {
                TxId = "EEER342",
                UserRefId = "GTII5769",
                Pair = "LTCUSD",
                Status = OrderState.Complete,
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
                Executed = "045643.23",
                VolumeExecuted = 2000
            });

            return orderList;
        }

        /// <summary>
        /// Returns orders of the user that have been filled/executed
        /// </summary>
        /// <param name="traderId"></param>
        /// <param name="txId"></param>
        /// <param name="includeTrades"></param>
        /// <returns></returns>
        public object QueryTrades(string orderId)
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
                Executed = "045643.23",
                VolumeExecuted = 2000
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
                Executed = "045643.23",
                VolumeExecuted = 2000
            });
            orderList.Add(new OrderRepresentation()
            {
                TxId = "EEER342",
                UserRefId = "GTII5769",
                Pair = "LTCUSD",
                Status = OrderState.Complete,
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
                Executed = "045643.23",
                VolumeExecuted = 2000
            });

            return orderList;
        }

        /// <summary>
        /// Recent trades info
        /// </summary>
        /// <param name="pair"></param>
        /// <param name="since"></param>
        /// <returns></returns>
        public IList<object> GetRecentTrades(string pair, string since)
        {
            List<TradeRecord> list = new List<TradeRecord>();
            TradeRecord entries = new TradeRecord(123.33m, 200, "Buy", "Limit", "", DateTime.UtcNow.ToString());
            for (int i = 0; i < 5; i++)
            {
                list.Add(entries);
            }
            TradeListRepresentation representation = new TradeListRepresentation("234", list, pair);
            List<object> trades=new List<object>();
            trades.Add(representation);
            return trades;
        }

        /// <summary>
        /// Trade volum request handler
        /// </summary>
        /// <param name="pair"> </param>
        /// <returns></returns>
        public TradeVolumeRepresentation TradeVolume(string pair)
        {
            TradeFeeRepresentation fees = new TradeFeeRepresentation(100m, 234m, 34.5m, 25.5m, 23.5m, 0.005m);
            TradeVolumeRepresentation response = new TradeVolumeRepresentation(fees, 1000, "ZUSD");
            return response;
        }
        
        public IList<CurrencyPair> GetTradeableCurrencyPairs()
        {
            List<CurrencyPair> pairs=new List<CurrencyPair>();
            pairs.Add(new CurrencyPair("BTC/USD","BTC","USD"));
            return pairs;
        }


        public TradeDetailsRepresentation GetTradeDetails(string traderId, string tradeId)
        {
            throw new NotImplementedException();
        }
    }
}

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
    public class TradeReadModel
    {
        #region Properties

        public string TradeId { get; private set; }
        public decimal Price { get; private set; }
        public decimal Volume { get; private set; }
        public DateTime ExecutionDateTime { get; private set; }
        public string CurrencyPair { get; private set; }
        public string BuyOrderId { get; private set; }
        public string SellOrderId { get; private set; }
        public string BuyTraderId { get; private set; }
        public string SellTraderId { get; private set; }

        #endregion

        public TradeReadModel()
        {
            
        }

        public TradeReadModel(string sellTraderId, string buyTraderId, string sellOrderId, string buyOrderId, string currencyPair, DateTime executionDateTime, decimal volume, decimal price, string tradeId)
        {
            SellTraderId = sellTraderId;
            BuyTraderId = buyTraderId;
            SellOrderId = sellOrderId;
            BuyOrderId = buyOrderId;
            CurrencyPair = currencyPair;
            ExecutionDateTime = executionDateTime;
            Volume = volume;
            Price = price;
            TradeId = tradeId;
        }
    }
}

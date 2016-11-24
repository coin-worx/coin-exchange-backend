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

namespace CoinExchange.Trades.Domain.Model.OrderMatchingEngine
{
    /// <summary>
    /// Contains the Limit and Depth OrderBook and their associated listeners for a currency pair that are linked to each 
    /// other at the Exchange
    /// </summary>
    [Serializable]
    public class ExchangeEssentials
    {
        public ExchangeEssentials()
        {
            
        }
        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="limitOrderBook"></param>
        /// <param name="depthOrderBook"></param>
        /// <param name="tradeListener"></param>
        /// <param name="orderListener"></param>
        /// <param name="depthListener"></param>
        /// <param name="bboListener"></param>
        public ExchangeEssentials(LimitOrderBook limitOrderBook, DepthOrderBook depthOrderBook, ITradeListener tradeListener,
            IOrderListener orderListener, IDepthListener depthListener, IBBOListener bboListener)
        {
            this.LimitOrderBook = limitOrderBook;
            this.DepthOrderBook = depthOrderBook;
            this.TradeListener = tradeListener;
            this.OrderListener = orderListener;
            this.DepthListener = depthListener;
            this.BBOListener = bboListener;
        }

        public void Update(ITradeListener tradeListener,
            IOrderListener orderListener, IDepthListener depthListener, IBBOListener bboListener)
        {
            this.TradeListener = tradeListener;
            this.OrderListener = orderListener;
            this.DepthListener = depthListener;
            this.BBOListener = bboListener;
        }

        /// <summary>
        /// Limit order Book
        /// </summary>
        public LimitOrderBook LimitOrderBook { get; private set; }

        /// <summary>
        /// DepthOrderBook
        /// </summary>
        public DepthOrderBook DepthOrderBook { get; private set; }

        /// <summary>
        /// Trade Listener event handler
        /// </summary>
        public ITradeListener TradeListener { get; private set; }

        /// <summary>
        /// Ordee Listener event handler
        /// </summary>
        public IOrderListener OrderListener { get; private set; }

        /// <summary>
        /// Depth Listener event handler
        /// </summary>
        public IDepthListener DepthListener { get; private set; }

        /// <summary>
        /// BBO Listener event handler
        /// </summary>
        public IBBOListener BBOListener { get; private set; }
    }
}

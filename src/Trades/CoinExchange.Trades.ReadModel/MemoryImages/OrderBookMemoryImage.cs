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
using CoinExchange.Common.Domain.Model;
using CoinExchange.Common.Utility;
using CoinExchange.Trades.Domain.Model.OrderAggregate;
using CoinExchange.Trades.Domain.Model.OrderMatchingEngine;
using Disruptor;

namespace CoinExchange.Trades.ReadModel.MemoryImages
{
    /// <summary>
    /// The memory image containing the OrderBook in memory
    /// </summary>
    public class OrderBookMemoryImage : IOrderBookMemoryImage
    {
        // Get the Current Logger
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger
        (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private List<string> _currencyPairs = new List<string>(); 
        private OrderRepresentationBookList _bidBookRepresentations = null;
        private OrderRepresentationBookList _askBookRepresentations = null;

        /// <summary>
        /// Default Constructor
        /// </summary>
        public OrderBookMemoryImage()
        {
            _bidBookRepresentations = new OrderRepresentationBookList();
            _askBookRepresentations = new OrderRepresentationBookList();
            InitializeCurrencyPairs();
            InitializeOrderBooksRepresentations();
            LimitOrderBookEvent.LimitOrderBookChanged += OnOrderBookChanged;
        }

        /// <summary>
        /// Initialize the set of currency pairs that the application is to support.
        /// </summary>
        private void InitializeCurrencyPairs()
        {
            _currencyPairs.Add(CurrencyConstants.BtcLtc);
            _currencyPairs.Add(CurrencyConstants.BtcLtcSeparated);
            _currencyPairs.Add(CurrencyConstants.XbtLtc);
            _currencyPairs.Add(CurrencyConstants.XbtLtcSeparated);
        }

        /// <summary>
        /// Initialize lists that will contain the OrderBook representations for all currency pairs
        /// </summary>
        private void InitializeOrderBooksRepresentations()
        {
            foreach (var currencyPair in _currencyPairs)
            {
                OrderRepresentationList bidRepresentationList = new OrderRepresentationList(currencyPair, OrderSide.Buy);
                OrderRepresentationList askRepresentationList = new OrderRepresentationList(currencyPair, OrderSide.Sell);

                _bidBookRepresentations.AddRecord(bidRepresentationList);
                _askBookRepresentations.AddRecord(askRepresentationList);
            }
        }

        /// <summary>
        /// Receives the Order Book from the output Disruptor
        /// </summary>
        /// <param name="orderBook"></param>
        public void OnOrderBookChanged(LimitOrderBook orderBook)
        {
            UpdateBids(orderBook.Bids);
            UpdateAsks(orderBook.Asks);
        }

        /// <summary>
        /// Update the bids in the bid book that just got received
        /// </summary>
        /// <param name="orderList"></param>
        private void UpdateBids(OrderList orderList)
        {
            var orderRepresentationList = GetOrderRepresentationList(_bidBookRepresentations, orderList);
            UpdateOrderRepresentationList(_bidBookRepresentations, orderRepresentationList, orderList);
        }

        /// <summary>
        /// Update the asks in the ask book that just got received
        /// </summary>
        /// <param name="orderList"></param>
        private void UpdateAsks(OrderList orderList)
        {
            var orderRepresentationList = GetOrderRepresentationList(_askBookRepresentations, orderList);
            UpdateOrderRepresentationList(_askBookRepresentations, orderRepresentationList, orderList);
        }

        /// <summary>
        /// Updates the OrderBookRepresentation. Used by both bid and aask representations
        /// </summary>
        private OrderRepresentationList GetOrderRepresentationList(OrderRepresentationBookList orderBookRepresentations, OrderList orderList)
        {
            foreach (OrderRepresentationList orderRepresentationList in orderBookRepresentations)
            {
                if (orderRepresentationList.CurrencyPair == orderList.CurrencyPair)
                {
                    return orderRepresentationList;
                }
            }
            return null;
        }

        /// <summary>
        /// Updates the OrderRepresentationlist with new values
        /// </summary>
        /// <returns></returns>
        private void UpdateOrderRepresentationList(OrderRepresentationBookList orderRepresentationBook, OrderRepresentationList orderRepresentationList, OrderList orderList)
        {
            // If the orderRepresentationLst for this currencyPair is already present, remove it
            if (orderRepresentationList != null)
            {
                orderRepresentationBook.Remove(orderRepresentationList.CurrencyPair);
            }
            // Initialize the Orderlist for this currencyPair
            orderRepresentationList = new OrderRepresentationList(orderList.CurrencyPair, orderList.OrderSide);

            // Add each order(Bid or Ask) in the correponding order list (Bids list or Asks list)
            foreach (Order order in orderList)
            {
                orderRepresentationList.AddRecord(order.OpenQuantity.Value, order.Price.Value,order.DateTime);
                Log.Debug("Order Representation added to Currency Pair : " + order.CurrencyPair + 
                          ". OpenQuantity: " + order.OpenQuantity.Value + " | Price: " + order.Price.Value);   
            }
            // Add the new orderList (order book for bids or asks) to the OrderBooksList of this memory image
            orderRepresentationBook.AddRecord(orderRepresentationList);
        }

        /// <summary>
        /// Represents the Bid Books
        /// </summary>
        public OrderRepresentationBookList BidBooks
        {
            get { return _bidBookRepresentations; }
        }

        public OrderRepresentationBookList AskBooks
        {
            get { return _askBookRepresentations; }
        }

        #region Implementation of IEventHandler<in byte[]>

        /// <summary>
        /// Event handler for the LimitOrderBook changed event published by the output disruptor
        /// </summary>
        /// <param name="data"></param>
        /// <param name="sequence"></param>
        /// <param name="endOfBatch"></param>
        public void OnNext(byte[] data, long sequence, bool endOfBatch)
        {
            object getObject = StreamConversion.ByteArrayToObject(data);
            if (getObject is LimitOrderBook)
            {
                this.OnOrderBookChanged(getObject as LimitOrderBook);
            }
            else
            {
                throw new FormatException("Expected a type of CoinExchange.TradesDomain.Model.OrderMatchingEngine.LimitOrderBook but was " + getObject.GetType());
            }
        }

        #endregion
    }
}
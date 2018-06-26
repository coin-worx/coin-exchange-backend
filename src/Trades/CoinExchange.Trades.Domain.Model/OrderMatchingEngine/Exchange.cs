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
using System.Timers;
using System.Threading.Tasks;
using CoinExchange.Common.Domain.Model;
using CoinExchange.Trades.Domain.Model.CurrencyPairAggregate;
using CoinExchange.Trades.Domain.Model.OrderAggregate;
using CoinExchange.Trades.Domain.Model.Services;
using Disruptor;

namespace CoinExchange.Trades.Domain.Model.OrderMatchingEngine
{
    /// <summary>
    /// Initializes andcontains all Order Books and forwards requests for submitting and cancelling orders
    /// </summary>
    [Serializable]
    public class Exchange : IEventHandler<InputPayload>
    {
        // Get the Current Logger
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger
            (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        //private string BtcLtc = CurrencyConstants.BtcLtc;
        //private string XbtLtc = CurrencyConstants.XbtLtc;
        private List<string> _currencyPairs = new List<string>();
        private ExchangeEssentialsList _exchangeEssentialsList = new ExchangeEssentialsList();
        [NonSerialized] 
        private Timer _snaphotTimer;

        /// <summary>
        /// Default Constructor
        /// </summary>
        public Exchange(IList<CurrencyPair> currencyPairs)
        {
            /*_currencyPairs.Add(CurrencyConstants.BtcLtc);
            _currencyPairs.Add(CurrencyConstants.XbtLtc);
            _currencyPairs.Add(CurrencyConstants.BtcLtcSeparated);
            _currencyPairs.Add(CurrencyConstants.XbtLtcSeparated);
            _currencyPairs.Add(CurrencyConstants.BtcDoge);
            _currencyPairs.Add(CurrencyConstants.XbtDoge);
            _currencyPairs.Add(CurrencyConstants.BtcDogeSeparated);
            _currencyPairs.Add(CurrencyConstants.XbtDogeSeparated);*/
            ExtractCurrencyPairs(currencyPairs);
            foreach (var currencyPair in _currencyPairs)
            {
                LimitOrderBook orderBook = new LimitOrderBook(currencyPair);
                DepthOrderBook depthOrderBook = new DepthOrderBook(currencyPair, 5);

                TradeListener tradeListener = new TradeListener();
                IOrderListener orderListener = new OrderListener();
                IOrderBookListener orderBookListener = new OrderBookListener();
                IBBOListener bboListener = new BBOListener();
                IDepthListener depthListener = new DepthListener();

                orderBook.OrderAccepted -= OnAccept;
                orderBook.OrderAccepted -= depthOrderBook.OnOrderAccepted;

                orderBook.OrderAccepted += OnAccept;
                orderBook.OrderAccepted += depthOrderBook.OnOrderAccepted;

                orderBook.OrderCancelled -= depthOrderBook.OnOrderCancelled;
                orderBook.OrderCancelled += depthOrderBook.OnOrderCancelled;

                orderBook.OrderBookChanged -= depthOrderBook.OnOrderBookChanged;
                orderBook.OrderBookChanged -= orderBookListener.OnOrderBookChanged;

                orderBook.OrderBookChanged += depthOrderBook.OnOrderBookChanged;
                orderBook.OrderBookChanged += orderBookListener.OnOrderBookChanged;

                orderBook.OrderChanged -= depthOrderBook.OnOrderChanged;
                orderBook.OrderChanged -= orderListener.OnOrderChanged;

                orderBook.OrderChanged += depthOrderBook.OnOrderChanged;
                orderBook.OrderChanged += orderListener.OnOrderChanged;

                orderBook.OrderFilled -= depthOrderBook.OnOrderFilled;
                orderBook.OrderFilled += depthOrderBook.OnOrderFilled;

                orderBook.TradeExecuted -= tradeListener.OnTrade;
                orderBook.TradeExecuted += tradeListener.OnTrade;

                depthOrderBook.BboChanged -= bboListener.OnBBOChange;
                depthOrderBook.DepthChanged -= depthListener.OnDepthChanged;

                depthOrderBook.BboChanged += bboListener.OnBBOChange;
                depthOrderBook.DepthChanged += depthListener.OnDepthChanged;

                _exchangeEssentialsList.AddEssentials(new ExchangeEssentials(orderBook, depthOrderBook, tradeListener,
                    orderListener, depthListener, bboListener));
            }
        }

        /// <summary>
        /// parameterized constructor
        /// </summary>
        public Exchange(IList<CurrencyPair> currencyPairs, ExchangeEssentialsList exchangeEssentialsList)
        {
            //_currencyPairs.Add(CurrencyConstants.BtcLtc);
            //_currencyPairs.Add(CurrencyConstants.XbtLtc);
            //_currencyPairs.Add(CurrencyConstants.BtcLtcSeparated);
            //_currencyPairs.Add(CurrencyConstants.XbtLtcSeparated);
            ExtractCurrencyPairs(currencyPairs);
            _exchangeEssentialsList = exchangeEssentialsList;
            foreach (var exchangeEssential in _exchangeEssentialsList)
            {
                //TradeListener tradeListener = new TradeListener();
                //IOrderListener orderListener = new OrderListener();
                IOrderBookListener orderBookListener = new OrderBookListener();
                //IBBOListener bboListener = new BBOListener();
                //IDepthListener depthListener = new DepthListener();

                exchangeEssential.LimitOrderBook.OrderAccepted -= OnAccept;
                exchangeEssential.LimitOrderBook.OrderAccepted -= exchangeEssential.DepthOrderBook.OnOrderAccepted;

                exchangeEssential.LimitOrderBook.OrderAccepted += OnAccept;
                exchangeEssential.LimitOrderBook.OrderAccepted += exchangeEssential.DepthOrderBook.OnOrderAccepted;

                exchangeEssential.LimitOrderBook.OrderCancelled -= exchangeEssential.DepthOrderBook.OnOrderCancelled;
                exchangeEssential.LimitOrderBook.OrderCancelled += exchangeEssential.DepthOrderBook.OnOrderCancelled;

                exchangeEssential.LimitOrderBook.OrderBookChanged -= exchangeEssential.DepthOrderBook.OnOrderBookChanged;
                exchangeEssential.LimitOrderBook.OrderBookChanged -= orderBookListener.OnOrderBookChanged;

                exchangeEssential.LimitOrderBook.OrderBookChanged += exchangeEssential.DepthOrderBook.OnOrderBookChanged;
                exchangeEssential.LimitOrderBook.OrderBookChanged += orderBookListener.OnOrderBookChanged;

                exchangeEssential.LimitOrderBook.OrderChanged -= exchangeEssential.DepthOrderBook.OnOrderChanged;
                exchangeEssential.LimitOrderBook.OrderChanged -= exchangeEssential.OrderListener.OnOrderChanged;

                exchangeEssential.LimitOrderBook.OrderChanged += exchangeEssential.DepthOrderBook.OnOrderChanged;
                exchangeEssential.LimitOrderBook.OrderChanged += exchangeEssential.OrderListener.OnOrderChanged;

                exchangeEssential.LimitOrderBook.OrderFilled -= exchangeEssential.DepthOrderBook.OnOrderFilled;
                exchangeEssential.LimitOrderBook.OrderFilled += exchangeEssential.DepthOrderBook.OnOrderFilled;

                exchangeEssential.LimitOrderBook.TradeExecuted -= exchangeEssential.TradeListener.OnTrade;
                exchangeEssential.LimitOrderBook.TradeExecuted += exchangeEssential.TradeListener.OnTrade;

                exchangeEssential.DepthOrderBook.BboChanged -= exchangeEssential.BBOListener.OnBBOChange;
                exchangeEssential.DepthOrderBook.DepthChanged -= exchangeEssential.DepthListener.OnDepthChanged;

                exchangeEssential.DepthOrderBook.BboChanged += exchangeEssential.BBOListener.OnBBOChange;
                exchangeEssential.DepthOrderBook.DepthChanged += exchangeEssential.DepthListener.OnDepthChanged;
                //exchangeEssential.Update(tradeListener,orderListener,depthListener,bboListener);
            }
        }

        /// <summary>
        /// Extracts a list of currency pairs as strings and adds to the _currencyPairs list
        /// </summary>
        /// <returns></returns>
        private void ExtractCurrencyPairs(IList<CurrencyPair> currencyPairs)
        {
            foreach (var currencyPair in currencyPairs)
            {
                _currencyPairs.Add(currencyPair.CurrencyPairName);
            }
        }

        /// <summary>
        /// initialize exchange after snaphot
        /// </summary>
        public void InitializeExchangeAfterSnaphot()
        {
            foreach (var exchangeEssential in _exchangeEssentialsList)
            {
                //IOrderBookListener orderBookListener = new OrderBookListener();
                //exchangeEssential.LimitOrderBook.OrderAccepted -= OnAccept;
                //exchangeEssential.LimitOrderBook.OrderAccepted -= exchangeEssential.DepthOrderBook.OnOrderAccepted;

                //exchangeEssential.LimitOrderBook.OrderAccepted += OnAccept;
                //exchangeEssential.LimitOrderBook.OrderAccepted += exchangeEssential.DepthOrderBook.OnOrderAccepted;

                //exchangeEssential.LimitOrderBook.OrderCancelled -= exchangeEssential.DepthOrderBook.OnOrderCancelled;
                //exchangeEssential.LimitOrderBook.OrderCancelled += exchangeEssential.DepthOrderBook.OnOrderCancelled;

                //exchangeEssential.LimitOrderBook.OrderBookChanged -= exchangeEssential.DepthOrderBook.OnOrderBookChanged;
                //exchangeEssential.LimitOrderBook.OrderBookChanged -= orderBookListener.OnOrderBookChanged;

                //exchangeEssential.LimitOrderBook.OrderBookChanged += exchangeEssential.DepthOrderBook.OnOrderBookChanged;
                //exchangeEssential.LimitOrderBook.OrderBookChanged += orderBookListener.OnOrderBookChanged;

                //exchangeEssential.LimitOrderBook.OrderChanged -= exchangeEssential.DepthOrderBook.OnOrderChanged;
                //exchangeEssential.LimitOrderBook.OrderChanged -= exchangeEssential.OrderListener.OnOrderChanged;

                //exchangeEssential.LimitOrderBook.OrderChanged += exchangeEssential.DepthOrderBook.OnOrderChanged;
                //exchangeEssential.LimitOrderBook.OrderChanged += exchangeEssential.OrderListener.OnOrderChanged;

                //exchangeEssential.LimitOrderBook.OrderFilled -= exchangeEssential.DepthOrderBook.OnOrderFilled;
                //exchangeEssential.LimitOrderBook.OrderFilled += exchangeEssential.DepthOrderBook.OnOrderFilled;

                //exchangeEssential.LimitOrderBook.TradeExecuted -= exchangeEssential.TradeListener.OnTrade;
                //exchangeEssential.LimitOrderBook.TradeExecuted += exchangeEssential.TradeListener.OnTrade;

                //exchangeEssential.DepthOrderBook.BboChanged -= exchangeEssential.BBOListener.OnBBOChange;
                //exchangeEssential.DepthOrderBook.DepthChanged -= exchangeEssential.DepthListener.OnDepthChanged;

                //exchangeEssential.DepthOrderBook.BboChanged += exchangeEssential.BBOListener.OnBBOChange;
                //exchangeEssential.DepthOrderBook.DepthChanged += exchangeEssential.DepthListener.OnDepthChanged;

                exchangeEssential.LimitOrderBook.PublishOrderBookState();
                exchangeEssential.DepthOrderBook.PublishDepth();
            }
        }

        #region Methods

        /// <summary>
        /// Places new order on the limit order book
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public bool PlaceNewOrder(Order order)
        {
            // Search for the order book to which this order should go to
            foreach (var exchangeEssentials in _exchangeEssentialsList)
            {
                if (exchangeEssentials.LimitOrderBook.CurrencyPair.Equals(order.CurrencyPair))
                {
                    exchangeEssentials.LimitOrderBook.PlaceOrder(order);
                    return true;
                }
            }
            /*switch (order.CurrencyPair.ToUpper())
            {
                case CurrencyConstants.BtcLtc:
                    return _exchangeEssentialsList.First().LimitOrderBook.PlaceOrder(order);
                case CurrencyConstants.XbtLtc:
                    return _exchangeEssentialsList.ToList()[1].LimitOrderBook.PlaceOrder(order);
                case CurrencyConstants.BtcLtcSeparated:
                    return _exchangeEssentialsList.ToList()[2].LimitOrderBook.PlaceOrder(order);
                case CurrencyConstants.XbtLtcSeparated:
                    return _exchangeEssentialsList.ToList()[3].LimitOrderBook.PlaceOrder(order);
            }*/
            return false;
        }

        /// <summary>
        /// Cancel the order with the given orderId
        /// </summary>
        /// <param name="orderCancellation"> </param>
        /// <returns></returns>
        public bool CancelOrder(OrderCancellation orderCancellation)
        {
            switch (orderCancellation.CurrencyPair)
            {
                case CurrencyConstants.BtcLtc:
                    return _exchangeEssentialsList.First().LimitOrderBook.CancelOrder(orderCancellation.OrderId);
                case CurrencyConstants.XbtLtc:
                    return _exchangeEssentialsList.ToList()[1].LimitOrderBook.CancelOrder(orderCancellation.OrderId);
                case CurrencyConstants.BtcLtcSeparated:
                    return _exchangeEssentialsList.ToList()[2].LimitOrderBook.CancelOrder(orderCancellation.OrderId);
                case CurrencyConstants.XbtLtcSeparated:
                    return _exchangeEssentialsList.ToList()[3].LimitOrderBook.CancelOrder(orderCancellation.OrderId);
            }
            return false;
        }

        /// <summary>
        /// Signifies that the Order has been accepted successfully
        /// </summary>
        public void OnAccept(Order order, Price matchedPrice, Volume matchedVolume)
        {
            Log.Debug("Order Accepted by Exchange. " + order.ToString());
            // Note: the notification to the client can be sent back from here
        }

        /// <summary>
        /// Turns on the replay mode and unsubscribes the OrderListener from listeneing to event while the replay mode is running
        /// </summary>
        public void TurnReplayModeOn()
        {
            ReplayService.TurnReplayModeOn(this);
            // Unsubscribe each order listener from each LimitOrderBook while replaying is in order
            foreach (ExchangeEssentials exchangeEssentials in ExchangeEssentials)
            {
                exchangeEssentials.LimitOrderBook.TradeExecuted -= exchangeEssentials.TradeListener.OnTrade;
                exchangeEssentials.LimitOrderBook.OrderChanged -= exchangeEssentials.DepthOrderBook.OnOrderChanged;
                exchangeEssentials.LimitOrderBook.OrderChanged -= exchangeEssentials.OrderListener.OnOrderChanged;
            }
        }

        /// <summary>
        /// Turns replay mode off 
        /// </summary>
        public void TurnReplayModeOff()
        {
            ReplayService.TurnReplayModeOff(this);
            foreach (ExchangeEssentials exchangeEssentials in ExchangeEssentials)
            {
                exchangeEssentials.LimitOrderBook.TradeExecuted += exchangeEssentials.TradeListener.OnTrade;
                exchangeEssentials.LimitOrderBook.OrderChanged += exchangeEssentials.DepthOrderBook.OnOrderChanged;
                exchangeEssentials.LimitOrderBook.OrderChanged += exchangeEssentials.OrderListener.OnOrderChanged;
            }
        }

        #endregion Methods

        /// <summary>
        /// ExchangeEssentials
        /// </summary>
        public ExchangeEssentialsList ExchangeEssentials
        {
            get { return _exchangeEssentialsList; }
            private set { _exchangeEssentialsList = value; }
        }

        /// <summary>
        /// Receives new order and cancel order requests from disruptor
        /// </summary>
        /// <param name="data"></param>
        /// <param name="sequence"></param>
        /// <param name="endOfBatch"></param>
        public void OnNext(InputPayload data, long sequence, bool endOfBatch)
        {
            if (data.IsOrder)
            {
                Order order = new Order();
                data.Order.MemberWiseClone(order);
                PlaceNewOrder(order);
            }
            else
            {
                OrderCancellation cancellation = new OrderCancellation();
                data.OrderCancellation.MemberWiseClone(cancellation);
                CancelOrder(cancellation);
            }
        }

        #region Exchange Snapshot

        /// <summary>
        /// Enable snapshots
        /// </summary>
        /// <param name="interval"></param>
        public void EnableSnaphots(double interval)
        {
            if (_snaphotTimer == null)
            {
                _snaphotTimer = new Timer(interval);
                _snaphotTimer.Elapsed += SnaphotTimer_Elapsed;
                _snaphotTimer.Enabled = true;
            }
        }

        /// <summary>
        /// Save snapshot
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SnaphotTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                _exchangeEssentialsList.LastSnapshotDateTime = DateTime.Now;
                ExchangeEssentialsSnapshortEvent.Raise(_exchangeEssentialsList);
            }
            catch (Exception exception)
            {
                if (Log.IsErrorEnabled)
                {
                    Log.Error("Snapshot Interval Triggered:",exception);
                }
            }
            
        }

        public void StopTimer()
        {
            if (_snaphotTimer != null)
            {
                _snaphotTimer.Stop();
                _snaphotTimer.Enabled = false;
                _snaphotTimer.Dispose();
            }
        }

        #endregion
    }
}

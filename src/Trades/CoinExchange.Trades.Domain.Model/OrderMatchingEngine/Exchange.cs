using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using System.Threading.Tasks;
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

        private string BitCoinUsd = "BTCUSD";
        private string XbtUsd = "XBTUSD";
        private List<string> _currencyPairs = new List<string>();
        private ExchangeEssentialsList _exchangeEssentialsList = new ExchangeEssentialsList();
        [NonSerialized] 
        private Timer _snaphotTimer;

        /// <summary>
        /// Default Constructor
        /// </summary>
        public Exchange()
        {
            _currencyPairs.Add(BitCoinUsd);
            _currencyPairs.Add(XbtUsd);
            _currencyPairs.Add("BTC/USD");
            _currencyPairs.Add("XBT/USD");
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
        public Exchange(ExchangeEssentialsList exchangeEssentialsList)
        {
            _currencyPairs.Add(BitCoinUsd);
            _currencyPairs.Add(XbtUsd);
            _currencyPairs.Add("BTC/USD");
            _currencyPairs.Add("XBT/USD");
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
            switch (order.CurrencyPair.ToUpper())
            {
                case "BTCUSD":
                    return _exchangeEssentialsList.First().LimitOrderBook.PlaceOrder(order);
                case "XBTUSD":
                    return _exchangeEssentialsList.ToList()[1].LimitOrderBook.PlaceOrder(order);
                case "BTC/USD":
                    return _exchangeEssentialsList.ToList()[2].LimitOrderBook.PlaceOrder(order);
                case "XBT/USD":
                    return _exchangeEssentialsList.ToList()[3].LimitOrderBook.PlaceOrder(order);
            }
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
                case "BTCUSD":
                    return _exchangeEssentialsList.First().LimitOrderBook.CancelOrder(orderCancellation.OrderId);
                case "XBTUSD":
                    return _exchangeEssentialsList.ToList()[1].LimitOrderBook.CancelOrder(orderCancellation.OrderId);
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

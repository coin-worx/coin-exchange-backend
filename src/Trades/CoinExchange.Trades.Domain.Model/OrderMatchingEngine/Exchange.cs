using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Trades.Domain.Model.OrderAggregate;

namespace CoinExchange.Trades.Domain.Model.OrderMatchingEngine
{
    /// <summary>
    /// Initializes all Order Books
    /// </summary>
    [Serializable]
    public class Exchange
    {
        // Get the Current Logger
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger
        (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private string BitCoinUsd = "BTCUSD";
        List<string> _currencyPairs = new List<string>();
        private ExchangeEssentialsList _exchangeEssentialsList = new ExchangeEssentialsList();
        private bool _isReplayMode = false;

        /// <summary>
        /// Default Constructor
        /// </summary>
        public Exchange()
        {
            _currencyPairs.Add(BitCoinUsd);
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

        #region Methods

        /// <summary>
        /// Places new order on the limit order book
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public bool PlaceNewOrder(Order order)
        {
            switch (order.CurrencyPair)
            {
                case "BTCUSD":
                    return _exchangeEssentialsList.First().LimitOrderBook.PlaceOrder(order);
            }
            return false;
        }

        /// <summary>
        /// Cancel the order with the given orderId
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public bool CancelOrder(OrderId orderId)
        {
            // ToDo: Ask Bilal to provide the currency pair as well here in order for the exchange to figure out which order
            // book to call
            return _exchangeEssentialsList.First().LimitOrderBook.CancelOrder(orderId);
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
            _isReplayMode = true;
            // Unsubscribe each order listener from each LimitOrderBook while replaying is in order
            foreach (ExchangeEssentials exchangeEssentials in ExchangeEssentials)
            {
                exchangeEssentials.LimitOrderBook.OrderChanged -= exchangeEssentials.DepthOrderBook.OnOrderChanged;
                exchangeEssentials.LimitOrderBook.OrderChanged -= exchangeEssentials.OrderListener.OnOrderChanged;
            }
        }

        /// <summary>
        /// Turns replay mode off 
        /// </summary>
        public void TurnReplayModeOff()
        {
            _isReplayMode = false;
            foreach (ExchangeEssentials exchangeEssentials in ExchangeEssentials)
            {
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
    }
}

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
        private LimitOrderBook _orderBook = null;
        private DepthOrderBook _depthOrderBook = null;
        private ITradeListener _tradeListener = null;
        private IOrderListener _orderListener = null;

        /// <summary>
        /// Default Constructor
        /// </summary>
        public Exchange()
        {
            _currencyPairs.Add(BitCoinUsd);
            foreach (var currencyPair in _currencyPairs)
            {
                _orderBook = new LimitOrderBook(currencyPair);
                _depthOrderBook = new DepthOrderBook(currencyPair, 5);

                _tradeListener = new TradeListener();
                IOrderListener orderListener = new OrderListener();
                IOrderBookListener orderBookListener = new OrderBookListener();
                IBBOListener bboListener = new BBOListener();
                IDepthListener depthListener = new DepthListener();

                _orderBook.OrderAccepted -= OnAccept;
                _orderBook.OrderAccepted -= _depthOrderBook.OnOrderAccepted;

                _orderBook.OrderAccepted += OnAccept;
                _orderBook.OrderAccepted += _depthOrderBook.OnOrderAccepted;

                _orderBook.OrderAccepted -= orderListener.OnOrderAccepted;
                _orderBook.OrderAccepted += orderListener.OnOrderAccepted;

                _orderBook.OrderCancelled -= _depthOrderBook.OnOrderCancelled;
                _orderBook.OrderCancelled += _depthOrderBook.OnOrderCancelled;

                _orderBook.OrderBookChanged -= _depthOrderBook.OnOrderBookChanged;
                _orderBook.OrderBookChanged -= orderBookListener.OnOrderBookChanged;

                _orderBook.OrderBookChanged += _depthOrderBook.OnOrderBookChanged;
                _orderBook.OrderBookChanged += orderBookListener.OnOrderBookChanged;

                _orderBook.OrderChanged -= _depthOrderBook.OnOrderChanged;
                _orderBook.OrderChanged -= orderListener.OnOrderChanged;

                _orderBook.OrderChanged += _depthOrderBook.OnOrderChanged;
                _orderBook.OrderChanged += orderListener.OnOrderChanged;

                _orderBook.OrderFilled -= _depthOrderBook.OnOrderFilled;
                _orderBook.OrderFilled += _depthOrderBook.OnOrderFilled;

                _orderBook.OrderFilled -= orderListener.OnOrderFilled;
                _orderBook.OrderFilled += orderListener.OnOrderFilled;

                _orderBook.TradeExecuted -= _tradeListener.OnTrade;
                _orderBook.TradeExecuted += _tradeListener.OnTrade;

                _depthOrderBook.BboChanged -= bboListener.OnBBOChange;
                _depthOrderBook.DepthChanged -= depthListener.OnDepthChanged;

                _depthOrderBook.BboChanged += bboListener.OnBBOChange;
                _depthOrderBook.DepthChanged += depthListener.OnDepthChanged;
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
            return _orderBook.PlaceOrder(order);
        }

        public bool CancelOrder(OrderId orderId)
        {
            return _orderBook.CancelOrder(orderId);
        }

        /// <summary>
        /// Signifies that the Order has been accepted successfully
        /// </summary>
        public void OnAccept(Order order, Price matchedPrice, Volume matchedVolume)
        {
            Log.Debug("Order Accepted by Exchange. " + order.ToString());
            // Note: the notification to the client can be sent back from here
        }

        #endregion Methods

        #region Properties

        public LimitOrderBook OrderBook { get { return _orderBook; } }

        public DepthOrderBook DepthOrderBook { get { return _depthOrderBook; } }

        public ITradeListener TradeListener { get { return _tradeListener; } }

        #endregion Properties
    }
}

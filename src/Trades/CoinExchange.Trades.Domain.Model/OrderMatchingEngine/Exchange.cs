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
    public class Exchange
    {
        // Get the Current Logger
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger
        (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private string BitCoinUsd = "BTCUSD";
        List<string> _currencyPairs = new List<string>(); 

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

                ITradeListener tradeListener = new TradeListener();
                IOrderListener orderListener = new OrderListener();
                IOrderBookListener orderBookListener = new OrderBookListener();
                IBBOListener bboListener = new BBOListener();
                IDepthListener depthListener = new DepthListener();

                orderBook.OrderAccepted += OnAccept;

                orderBook.OrderBookChanged += depthOrderBook.OnOrderBookChanged;
                orderBook.OrderBookChanged += orderBookListener.OnOrderBookChanged;

                orderBook.OrderChanged += depthOrderBook.OnOrderChanged;
                orderBook.OrderChanged += orderListener.OnOrderChanged;

                orderBook.OrderFilled += depthOrderBook.OrderFilled;

                orderBook.TradeExecuted += depthOrderBook.OnTrade;
                orderBook.TradeExecuted += tradeListener.OnTrade;

                depthOrderBook.BboChanged += bboListener.OnBBOChange;
                depthOrderBook.DepthChanged += depthListener.OnDepthChanged;
            }
        }

        #region Methods

        /// <summary>
        /// Signifies that the Order has been accepted successfully
        /// </summary>
        public void OnAccept(Order order)
        {
            Log.Debug("Order Accepted by Exchange. " + order.ToString());
            // ToDo: Send the notification back to the client
        }

        #endregion Methods
    }
}

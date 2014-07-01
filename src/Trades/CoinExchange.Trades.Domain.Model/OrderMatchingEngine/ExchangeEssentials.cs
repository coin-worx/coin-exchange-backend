using System;
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

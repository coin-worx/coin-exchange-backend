using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Trades.Domain.Model.Order;

namespace CoinExchange.Trades.Domain.Model.OrderMatchingEngine
{
    /// <summary>
    /// Contains the Depth levels for each price in the market
    /// </summary>
    public class Depth
    {
        #region Private fields

        private string _currencyPair = string.Empty;
        private int _size = 0;
        private DepthLevel _bestBid = null;
        private DepthLevel _bestAsk = null;
        private DepthLevel _lastBid = null;
        private DepthLevel _lastAsk = null;

        private DepthLevel[] _bid;
        private Array array = new Array[100];

        // Gets one past the last ask level
        private DepthLevel _end = null;

        private int _lastChangeId = 0;
        private int _lastPublishedChangeId = 0;

        #endregion Private Fields

        /// <summary>
        /// Default constructor
        /// </summary>
        public Depth(string currencyPair, int size)
        {
            _currencyPair = currencyPair;
            _size = size;
        }

        #region Methods

        /// <summary>
        /// Add Order to the Depth
        /// </summary>
        /// <returns></returns>
        public void AddOrder(Price price, Volume volume, OrderSide orderSide)
        {
            int changeIdCopy = _lastChangeId;
            // ToDo: Continued
        }

        /// <summary>
        /// Ignore future fill quantity on a side, due to a match at accept time for an order
        /// </summary>
        /// <param name="price"></param>
        /// <param name="qty"></param>
        /// <param name="orderSide"></param>
        public void IgnoreFillQuantity(Price price, Volume qty, OrderSide orderSide)
        {
            
        }

        /// <summary>
        /// Handle an order fill
        /// </summary>
        /// <param name="price"></param>
        /// <param name="qty"></param>
        /// <param name="filled"></param>
        /// <param name="orderSide"></param>
        public void FillOrder(Price price, Volume qty,bool filled,  OrderSide orderSide)
        {

        }

        /// <summary>
        /// Cancel or fill an order
        /// </summary>
        /// <param name="price"></param>
        /// <param name="volume"></param>
        /// <param name="is_bid"></param>
        /// <returns></returns>
        public bool CloseOrder(Price price, Volume volume, bool is_bid)
        {
            return false;
        }

        /// <summary>
        /// Change quantity of an order
        /// </summary>
        /// <param name="price"></param>
        /// <param name="qtyDelta"></param>
        /// <param name="orderSide"></param>
        public void ChangeOrderQuantity(Price price, int qtyDelta, OrderSide orderSide)
        {
            
        }

        /// <summary>
        /// Replace an Order
        /// </summary>
        /// <param name="current_price"></param>
        /// <param name="new_price"></param>
        /// <param name="currentQuantity"></param>
        /// <param name="newQuantity"></param>
        /// <param name="orderSide"></param>
        public void ReplaceOrder(Price current_price, Price new_price, Volume currentQuantity, Volume newQuantity,
                                  OrderSide orderSide)
        {

        }

        /// <summary>
        /// Does this depth need bid restoration after level erasure
        /// </summary>
        /// <param name="restorationPrice">The price to restore after (out)</param>
        /// <returns>True if restoration is needed (previously was full)</returns>
        public bool NeedsBidRestoration(Price restorationPrice)
        {
            return false;
        }

        /// <summary>
        /// Does this depth need ask restoration after level erasure
        /// </summary>
        /// <param name="restorationPrice">The price to restore after (out)</param>
        /// <returns> true if restoration is needed (previously was full)</returns>
        public bool NeedsAskRestoration(Price restorationPrice)
        {
            return false;
        }

        /// <summary>
        /// Has the depth changed since the last publish
        /// </summary>
        /// <returns></returns>
        public bool Changed()
        {
            return false;
        }

        /// <summary>
        /// note the ID of last published change
        /// </summary>
        public void Published()
        {
      
        }

        #region Level Finders

        /// <summary>
        /// Find the level specified by price, create new if does not exist
        /// </summary>
        public void FindLevel()
        {
            
        }

        #endregion Level Finders

        #endregion Methods

        #region Properties

        /// <summary>
        /// CurrencyPair
        /// </summary>
        public string CurrencyPair
        {
            get
            {
                return _currencyPair;
            } 
            set
            {
                _currencyPair = value;
            }
        }

        /// <summary>
        /// Last Change's Id
        /// </summary>
        public int LastChangeId
        {
            get
            {
                return _lastChangeId;
            }
            set
            {
                _lastChangeId = value;
            }
        }

        /// <summary>
        /// Last Published Change's Id
        /// </summary>
        public int LastPublishedChangeId
        {
            get
            {
                return _lastPublishedChangeId;
            }
            set
            {
                _lastPublishedChangeId = value;
            }
        }

        #endregion Properties
    }
}

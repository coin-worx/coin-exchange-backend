using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinExchange.Trades.Domain.Model.OrderMatchingEngine
{
    /// <summary>
    /// Handles the depths for the price levels in the Order Book
    /// </summary>
    public class DepthOrderBook
    {
        // ToDo: Need to implement the depth feature of the DethOrderBook
        // ToDo: Need to figure whether to inherit the DepthOrderBook from OrderBook, link it with OrderBook only, or put this
        // functionality into the OrderBook and no DepthBook be created separately.

        private string _currencyPair = string.Empty;

        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="currencyPair"></param>
        public DepthOrderBook(string currencyPair)
        {
            _currencyPair = currencyPair;
        }

        #region Methods

        /// <summary>
        /// After an Order is accepted in the LimitOrderBook, adds the new order's attributes to the corresponding depth level
        /// </summary>
        /// <returns></returns>
        public bool OrderAccepted(Order.Order order)
        {
            return false;
        }

        /// <summary>
        /// After an Order is filled in the LimitOrderBook, adds the new order's attributes to the corresponding depth level
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public bool OrderFilled(Order.Order order)
        {
            return false;
        }

        /// <summary>
        /// After an Order is cancelled in the LimitOrderBook, adds the new order's attributes to the corresponding depth level
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public bool OrderCancel(Order.Order order)
        {
            return false;
        }

        /// <summary>
        /// After an Order is replaced in the LimitOrderBook, adds the new order's attributes to the corresponding depth level
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public bool OrderReplace(Order.Order order)
        {
            return false;
        }

        /// <summary>
        /// After the OrderBook was updated, see if the depth we track was effected
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public bool OrderBookUpdated(Order.Order order)
        {
            return false;
        }

        #endregion Methods

        #region Properties

        /// <summary>
        /// Currency Pair of this book
        /// </summary>
        public string CurrencyPair
        {
            get { return _currencyPair; }
        }

        #endregion Properties
    }
}

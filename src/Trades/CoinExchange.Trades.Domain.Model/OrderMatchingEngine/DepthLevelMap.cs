using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Trades.Domain.Model.Order;

namespace CoinExchange.Trades.Domain.Model.OrderMatchingEngine
{
    public class DescendingOrderComparer : IComparer<decimal>
    {
        #region Implementation of IComparer<in decimal>

        public int Compare(decimal x, decimal y)
        {
            // Use the default comparer to do the original comparison for datetimes
            int ascendingResult = Comparer<decimal>.Default.Compare(x, y);

            // Turn the result around
            return 0 - ascendingResult;
        }

        #endregion
    }

    /// <summary>
    /// Contains the depth levels for each price in the book
    /// </summary>
    public class DepthLevelMap : IEnumerable<KeyValuePair<decimal, DepthLevel>>
    {
        // Get the Current Logger
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger
        (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private string _currencyPair = string.Empty;
        private OrderSide _orderSide;
        private SortedList<decimal, DepthLevel> _depthLevels = null;
 
        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="currencyPair"></param>
        /// <param name="orderSide"></param>
        public DepthLevelMap(string currencyPair, OrderSide orderSide)
        {
            _currencyPair = currencyPair;
            _orderSide = orderSide;

            if (orderSide == OrderSide.Buy)
            {
                _depthLevels = new SortedList<decimal, DepthLevel>(new DescendingOrderComparer());
            }
            else
            {
                _depthLevels = new SortedList<decimal, DepthLevel>();
            }
        }

        /// <summary>
        /// Add to the depth Level
        /// </summary>
        /// <param name="price"></param>
        /// <param name="depthLevel"> </param>
        /// <returns></returns>
        internal bool AddLevel(Price price, DepthLevel depthLevel)
        {
            try
            {
                _depthLevels.Add(price.Value, depthLevel);
                return true;
            }
            catch (Exception exception)
            {
                Log.Error("Could not add DepthLevel to DepthLevelMap. " + exception);
                return false;
            }
        }

        /// <summary>
        /// Add to the depth Level
        /// </summary>
        /// <param name="price"></param>
        /// <returns></returns>
        internal bool RemoveLevel(Price price)
        {
            try
            {
                if (_depthLevels.ContainsKey(price.Value))
                {
                    _depthLevels.Remove(price.Value);
                    return true;
                }
            }
            catch (Exception exception)
            {
                Log.Error("Could not add DepthLevel to DepthLevelMap. " + exception);
            }
            return false;
        }

        #region Proeprties

        /// <summary>
        /// The CurrencyPair for which this list specifies the OrderList
        /// </summary>
        // ToDo: Need to use the object for the 'CurrencyPair' class instead of string after Bilal has finished editing
        public string CurrencyPair
        {
            get { return _currencyPair; }
        }

        /// <summary>
        /// The CurrencyPair for which this list specifies the OrderList
        /// </summary>
        public OrderSide OrderSide
        {
            get { return _orderSide; }
        }

        public SortedList<decimal, DepthLevel> DepthLevels
        {
            get { return _depthLevels; }
        }

        #endregion Properties

        #region Implementation of IEnumerable

        public IEnumerator<KeyValuePair<decimal, DepthLevel>> GetEnumerator()
        {
            foreach (KeyValuePair<decimal, DepthLevel> keyValPair in _depthLevels)
            {
                // Lets check for end of list (its bad code since we used arrays)
                if (keyValPair.Value == null)
                {
                    break;
                }

                // Return the current element and then on next function call 
                // resume from next element rather than starting all over again;
                yield return keyValPair;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Trades.Domain.Model.OrderAggregate;

namespace CoinExchange.Trades.ReadModel.MemoryImages
{
    /// <summary>
    /// Represents the OrderBook in a simplified form to be shown on the UI. Contains
    /// 1. Volume
    /// 2. Price
    /// in every slot
    /// </summary>
    public class OrderRepresentationList : IEnumerable<Tuple<decimal, decimal>>
    {
        // Get the Current Logger
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger
        (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Contains slots as tuples and each tuple represents:
        /// 1. Volume
        /// 2. Price
        /// </summary>
        private List<Tuple<decimal, decimal>> _orderRecordList = new List<Tuple<decimal, decimal>>();

        private string _currencyPair = null;
        private OrderSide _orderSide;

        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="currencyPair"></param>
        /// <param name="orderSide"> </param>
        public OrderRepresentationList(string currencyPair, OrderSide orderSide)
        {
            _currencyPair = currencyPair;
            _orderSide = orderSide;
        }
  
        /// <summary>
        /// Add an Order to the List
        /// </summary>
        /// <returns></returns>
        internal bool AddRecord(/*string currencyPair, OrderSide orderSide, */decimal volume, decimal price)
        {
            /*if (volume != 0 && currencyPair == _currencyPair && orderSide == _orderSide)
            {*/
                _orderRecordList.Add(new Tuple<decimal, decimal>(volume, price));

                Log.Debug("New Order record added: CurrencyPair = " + _currencyPair + " | Volume = " + volume + " | " +
                          "Price = " + price);
                return true;
            //}
            // Otherwise, log the error and return false
            Log.Debug("Wrong/non-matching Currencypair or OrderSide provided.");
            
            return false;
        }

        /// <summary>
        /// Updates the price and volume at the given index
        /// </summary>
        /// <param name="index"></param>
        /// <param name="volume"></param>
        /// <param name="price"></param>
        /// <returns></returns>
        public bool UpdateAtIndex(int index, decimal volume, decimal price)
        {
            if (index < _orderRecordList.Count)
            {
                _orderRecordList[index] = new Tuple<decimal, decimal>(volume, price);
                return true;
            }
            else
            {
                throw new IndexOutOfRangeException();
            }
            return false;
        }

        /// <summary>
        /// CurrencyPair
        /// </summary>
        public string CurrencyPair
        {
            get { return _currencyPair; }
        }

        /// <summary>
        /// Orderside
        /// </summary>
        public OrderSide OrderSide
        {
            get { return _orderSide; }
        }

        #region Implementation of IEnumerable

        /// <summary>
        /// GetEnumerator(specific)
        /// </summary>
        /// <returns></returns>
        IEnumerator<Tuple<decimal, decimal>> IEnumerable<Tuple<decimal, decimal>>.GetEnumerator()
        {
            foreach (Tuple<decimal, decimal> orderStats in _orderRecordList)
            {
                // Lets check for end of list (its bad code since we used arrays)
                if (orderStats == null)
                {
                    break;
                }

                // Return the current element and then on next function call 
                // resume from next element rather than starting all over again;
                yield return orderStats;
            }
        }

        /// <summary>
        /// GetEnumerator(generic)
        /// </summary>
        /// <returns></returns>
        public IEnumerator GetEnumerator()
        {
            foreach (Tuple<decimal, decimal> orderStats in _orderRecordList)
            {
                // Lets check for end of list (its bad code since we used arrays)
                if (orderStats == null)
                {
                    break;
                }

                // Return the current element and then on next function call 
                // resume from next element rather than starting all over again;
                yield return orderStats;
            }
        }

        #endregion
    }
}

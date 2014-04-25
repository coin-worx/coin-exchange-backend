using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Trades.Domain.Model.OrderAggregate;

namespace CoinExchange.Trades.ReadModel.MemoryImages
{
    /// <summary>
    /// Represents the list of OrderRepresentations as Volume and Price
    /// </summary>
    public class OrderRepresentationBookList : IEnumerable<OrderRepresentationList>
    {
        // Get the Current Logger
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger
        (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Contains slots as tuples and each tuple represents:
        /// 1. Volume
        /// 2. Price
        /// </summary>
        private List<OrderRepresentationList> _orderRepresentationLists = new List<OrderRepresentationList>();

        /// <summary>
        /// Add an Order to the List
        /// </summary>
        /// <returns></returns>
        internal bool AddRecord(OrderRepresentationList orderRepresentationList)
        {
            _orderRepresentationLists.Add(orderRepresentationList);
            return true;
        }

        /// <summary>
        /// Removes an OrderRepresentation given the currencyPair and OrderSide
        /// </summary>
        /// <param name="currencyPair"></param>
        /// <param name="orderSide"></param>
        /// <returns></returns>
        internal bool Remove(string currencyPair)
        {
            foreach (var orderRepresentationList in _orderRepresentationLists)
            {
                if (orderRepresentationList.CurrencyPair == currencyPair)
                {
                    _orderRepresentationLists.Remove(orderRepresentationList);
                    return true;
                }
            }
            return false;
        }

        #region Implementation of IEnumerable

        /// <summary>
        /// GetEnumerator(specific)
        /// </summary>
        /// <returns></returns>
        IEnumerator<OrderRepresentationList> IEnumerable<OrderRepresentationList>.GetEnumerator()
        {
            foreach (OrderRepresentationList orderRepresentationList in _orderRepresentationLists)
            {
                // Lets check for end of list (its bad code since we used arrays)
                if (orderRepresentationList == null)
                {
                    break;
                }

                // Return the current element and then on next function call 
                // resume from next element rather than starting all over again;
                yield return orderRepresentationList;
            }
        }

        /// <summary>
        /// GetEnumerator(generic)
        /// </summary>
        /// <returns></returns>
        public IEnumerator GetEnumerator()
        {
            foreach (OrderRepresentationList orderRepresentations in _orderRepresentationLists)
            {
                // Lets check for end of list (its bad code since we used arrays)
                if (orderRepresentations == null)
                {
                    break;
                }

                // Return the current element and then on next function call 
                // resume from next element rather than starting all over again;
                yield return orderRepresentations;
            }
        }

        #endregion
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Trades.Domain.Model.OrderAggregate;

namespace CoinExchange.Trades.Domain.Model.OrderMatchingEngine
{
    /// <summary>
    /// Contains a list of LimitOrderBooks
    /// </summary>
    [Serializable]
    public class ExchangeEssentialsList : IEnumerable<ExchangeEssentials>
    {
        // Get the Current Logger
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger
        (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private List<ExchangeEssentials> _orderBooksList = new List<ExchangeEssentials>();
        public DateTime LastSnapshotDateTime = DateTime.Now;

        /// <summary>
        /// Default Constructor
        /// </summary>
        public ExchangeEssentialsList()
        {
            
        }

        #region Methods

        /// <summary>
        /// Addds a LimitOrderBook to the Order books list
        /// </summary>
        /// <param name="exchangeEssentials"></param>
        internal bool AddEssentials(ExchangeEssentials exchangeEssentials)
        {
            if (!_orderBooksList.Contains(exchangeEssentials))
            {
                _orderBooksList.Add(exchangeEssentials);
                return true;
            }
            else
            {
                Log.Debug("Exchange essentials List already contains order book for: " + exchangeEssentials.LimitOrderBook.CurrencyPair);
            }
            return false;
        }

        /// <summary>
        /// Removes the order book from the list
        /// </summary>
        /// <returns></returns>
        internal bool RemoveEssentials(ExchangeEssentials exchangeEssentials)
        {
            if (_orderBooksList.Contains(exchangeEssentials))
            {
                _orderBooksList.Remove(exchangeEssentials);
                return true;
            }
            else
            {
                Log.Debug("Order book not present in ExchangeEssentialsList for: " + exchangeEssentials.LimitOrderBook.CurrencyPair);
            }
            return false;
        }

        #endregion Methods

        #region Implementation of IEnumerable

        public IEnumerator<ExchangeEssentials> GetEnumerator()
        {
            foreach (ExchangeEssentials order in _orderBooksList)
            {
                if (order == null)
                {
                    break;
                }

                // Return the current element and then on next function call 
                // resume from next element rather than starting all over again;
                yield return order;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinExchange.Trades.Domain.Model.TradeAggregate
{
    /// <summary>
    /// Represents a list of Trades
    /// </summary>
    public class TradeList : IEnumerable<Trade>
    {
        // Get the Current Logger
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger
        (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private List<Trade> _tradeList = new List<Trade>();

        private string _currencyPair = null;

        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="currencyPair"></param>
        public TradeList(string currencyPair)
        {
            _currencyPair = currencyPair;
        }
  
        /// <summary>
        /// Add a Trade to the List
        /// </summary>
        /// <returns></returns>
        internal bool Add(Trade trade)
        {
            // Check whether the incoming Trade is of the same CurrencyPair and Side for which this list was created
            if (trade != null && trade.CurrencyPair == _currencyPair)
            {
                // If yes, add the Trade, sort the list and log the details
                _tradeList.Add(trade);

                Log.Debug("Trade added to currency pair: " + _currencyPair.ToString(CultureInfo.InvariantCulture) + 
                    ". Trade = " + trade.ToString());
                return true;
            }
            // Otherwise, log the error and return false
            Log.Debug("Trade could not be added as currency pairs don't match.");
            
            return false;
        }

        /// <summary>
        /// The CurrencyPair for which this list specifies the TradeList
        /// </summary>
        public string CurrencyPair
        {
            get { return _currencyPair; }
        }

        #region Implementation of IEnumerable

        IEnumerator<Trade> IEnumerable<Trade>.GetEnumerator()
        {
            foreach (Trade trade in _tradeList)
            {
                // Lets check for end of list (its bad code since we used arrays)
                if (trade == null)
                {
                    break;
                }

                // Return the current element and then on next function call 
                // resume from next element rather than starting all over again;
                yield return trade;
            }
        }

        public IEnumerator GetEnumerator()
        {
            foreach (Trade trade in _tradeList)
            {
                if (trade == null)
                {
                    break;
                }

                // Return the current element and then on next function call 
                // resume from next element rather than starting all over again;
                yield return trade;
            }
        }

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinExchange.Trades.Domain.Model.OrderMatchingEngine
{
    /// <summary>
    /// Contains the Depth levels for each price in the market
    /// </summary>
    public class Depth
    {
        private string _currencyPair = string.Empty;

        /// <summary>
        /// Default constructor
        /// </summary>
        public Depth(string currencyPair)
        {
            _currencyPair = currencyPair;
        }

        #region Methods

        /// <summary>
        /// Add Order to the Depth
        /// </summary>
        /// <returns></returns>
        public bool AddOrder(Order.Order order)
        {
            return false;
        }

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

        #endregion Properties
    }
}

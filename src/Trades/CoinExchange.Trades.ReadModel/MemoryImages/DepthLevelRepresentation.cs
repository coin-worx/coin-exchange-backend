using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinExchange.Trades.ReadModel.MemoryImages
{
    /// <summary>
    /// Represents the Depth level in a form as: Price | Volume | Number of Orders
    /// </summary>
    public class DepthLevelRepresentation
    {
        private string _currencyPair = string.Empty;
        private decimal _price = 0;
        private decimal _volume = 0;
        private int _orderCount = 0;

        /// <summary>
        /// Default Constructor
        /// </summary>
        public DepthLevelRepresentation(string currencyPair, decimal price, decimal volume, int orderCount)
        {
            _currencyPair = currencyPair;
            _price = price;
            _volume = volume;
            _orderCount = orderCount;
        }

        /// <summary>
        /// CurrencyPair
        /// </summary>
        public string CurrencyPair
        {
            get { return _currencyPair; }
        }

        /// <summary>
        /// Volume
        /// </summary>
        public decimal Volume
        {
            get { return _volume; }
        }

        /// <summary>
        /// Price
        /// </summary>
        public decimal Price
        {
            get { return _price; }
        }


        public int OrderCount
        {
            get { return _orderCount; }
        }
    }
}

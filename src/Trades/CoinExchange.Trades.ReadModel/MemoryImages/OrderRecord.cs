using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CoinExchange.Trades.ReadModel.MemoryImages
{
    /// <summary>
    /// VO for containing Price and Volume in orderbook
    /// </summary>
    public class OrderRecord
    {
        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="price"></param>
        /// <param name="volume"></param>
        public OrderRecord(decimal price, decimal volume,DateTime dateTime)
        {
            Price = price;
            Volume = volume;
            DateTime = dateTime;
        }
        public decimal Volume { get; private set; }
        public decimal Price { get; private set; }
        public DateTime DateTime { get; private set; }

    }
}

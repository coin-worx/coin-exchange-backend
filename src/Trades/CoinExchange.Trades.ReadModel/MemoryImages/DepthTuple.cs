using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CoinExchange.Trades.ReadModel.MemoryImages
{
    /// <summary>
    /// VO to represent Depth Volume,Price & OrderCount
    /// </summary>
    public class DepthTuple
    {
        public decimal Volume { get; private set; }
        public decimal Price { get; private set; }
        public int OrderCount { get; private set; }

        /// <summary>
        /// default constructor
        /// </summary>
        /// <param name="orderCount"></param>
        /// <param name="price"></param>
        /// <param name="volume"></param>
        public DepthTuple(decimal volume, decimal price, int orderCount)
        {
            OrderCount = orderCount;
            Price = price;
            Volume = volume;
        }
    }
}

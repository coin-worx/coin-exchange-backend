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
        [JsonProperty(PropertyName = "Volume")]
        public decimal Item1 { get; private set; }
        [JsonProperty(PropertyName = "Price")]
        public decimal Item2 { get; private set; }
        [JsonProperty(PropertyName = "OrderCount")]
        public int Item3 { get; private set; }

        /// <summary>
        /// default constructor
        /// </summary>
        /// <param name="orderCount"></param>
        /// <param name="price"></param>
        /// <param name="volume"></param>
        public DepthTuple(decimal volume, decimal price, int orderCount)
        {
            Item3 = orderCount;
            Item2 = price;
            Item1 = volume;
        }
    }
}

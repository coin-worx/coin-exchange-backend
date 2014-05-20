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
        public OrderRecord(decimal price, decimal volume)
        {
            Item2 = price;
            Item1 = volume;
        }
        [JsonProperty(PropertyName = "Volume")]
        public decimal Item1 { get; private set; }
        [JsonProperty(PropertyName = "Price")]
        public decimal Item2 { get; private set; }

    }
}

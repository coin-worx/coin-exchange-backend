using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinExchange.Common.Domain.Model
{
    /// <summary>
    /// Contains constants related to bitcoin
    /// </summary>
    public class BitcoinConstants
    {
        /// <summary>
        /// When the transaction from bitcoin is received. This is a reserved keyword in bitcoin client
        /// </summary>
        public const string ReceiveCategory = "receive";
    }
}

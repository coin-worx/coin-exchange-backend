using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinExchange.Trades.Port.Adapter.Rest.DTOs.Order
{
    /// <summary>
    /// Contains the Parameters for the orders/openorders Http request action
    /// </summary>
    public class QueryOpenOrdersParams
    {
        private bool _includeTrades = false;
        private string _userRefId = string.Empty;

        /// <summary>
        /// Parameterized Constructor
        /// </summary>
        /// <param name="includeTrades"></param>
        /// <param name="userRefId"> </param>
        public QueryOpenOrdersParams(bool includeTrades, string userRefId)
        {
            _includeTrades = includeTrades;
            _userRefId = userRefId;
        }

        /// <summary>
        /// Should the reposnce include Trades or not
        /// </summary>
        public bool IncludeTrades { get { return _includeTrades; } }


        /// <summary>
        /// User Reference ID
        /// </summary>
        public string UserRefId { get { return _userRefId; } }
    }
}

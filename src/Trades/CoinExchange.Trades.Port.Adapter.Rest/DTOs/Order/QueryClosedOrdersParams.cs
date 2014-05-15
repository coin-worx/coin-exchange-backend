using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinExchange.Trades.Port.Adapter.Rest.DTOs.Order
{
    /// <summary>
    /// Contians params for orders/closedorders Http request action
    /// </summary>
    public class QueryClosedOrdersParams
    {
        private bool _includeTrades = false;
        private string _startTime = string.Empty;
        private string _endTime = string.Empty;
        
        public QueryClosedOrdersParams(bool includeTrades, string startTime, string endTime)
        {
            _includeTrades = includeTrades;
            _startTime = startTime;
            _endTime = endTime;
        }

        /// <summary>
        /// Cutom tostring method
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("Closed Order Params, IncludeTrades={0},StartTime={1},EndTimr={2}", IncludeTrades,
                StartTime, EndTime);
        }

        /// <summary>
        /// IncludeTrades
        /// </summary>
        public bool IncludeTrades { get { return _includeTrades; } }

        /// <summary>
        /// StartTime
        /// </summary>
        public string StartTime { get { return _startTime; } }

        /// <summary>
        /// EndTime
        /// </summary>
        public string EndTime { get { return _endTime; } }
    }
}

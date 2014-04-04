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
        private string _userRefId = string.Empty;
        private string _startTime = string.Empty;
        private string _endTime = string.Empty;
        private string _offset = string.Empty;
        private string _closeTime = string.Empty;

        public QueryClosedOrdersParams(bool includeTrades, string userRefId, string startTime, string endTime, string offset,
            string closeTime)
        {
            _includeTrades = includeTrades;
            _userRefId = userRefId;
            _startTime = startTime;
            _endTime = endTime;
            _offset = offset;
            _closeTime = closeTime;
        }

        /// <summary>
        /// IncludeTrades
        /// </summary>
        public bool IncludeTrades { get { return _includeTrades; } }

        /// <summary>
        /// UserRefId
        /// </summary>
        public string UserRefId { get { return _userRefId; } }

        /// <summary>
        /// StartTime
        /// </summary>
        public string StartTime { get { return _startTime; } }

        /// <summary>
        /// EndTime
        /// </summary>
        public string EndTime { get { return _endTime; } }

        /// <summary>
        /// OffSet
        /// </summary>
        public string Offset { get { return _offset; } }

        /// <summary>
        /// CloseTime
        /// </summary>
        public string CloseTime { get { return _closeTime; } }
    }
}

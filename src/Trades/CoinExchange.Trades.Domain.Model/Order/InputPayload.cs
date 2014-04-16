using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinExchange.Trades.Domain.Model.Order
{
    /// <summary>
    /// Input for disruptor for new order and cancel order requests
    /// </summary>
    public class InputPayload
    {
        private Order _order;
        private CancelOrder _cancelOrder;
        private bool _isOrder;//if true means it is a new order, false=>cancel order

        public Order Order
        {
            get { return _order; }
            set
            {
                _order = value;
            }
        }

        public CancelOrder CancelOrder
        {
            get { return _cancelOrder; }
            set
            {
               _cancelOrder = value;
            }
        }
        
        public bool IsOrder
        {
            get { return _isOrder; }
            set { _isOrder = value; }
        }
    }
}

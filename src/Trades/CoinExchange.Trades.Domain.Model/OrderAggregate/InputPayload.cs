namespace CoinExchange.Trades.Domain.Model.OrderAggregate
{
    /// <summary>
    /// Input for disruptor for new order and cancel order requests
    /// </summary>
    public class InputPayload
    {
        private Order _order;
        private OrderCancellation _orderCancellation;
        private bool _isOrder;//if true means it is a new order, false=>cancel order

        public Order Order
        {
            get { return _order; }
            set
            {
                _order = value;
            }
        }

        public OrderCancellation OrderCancellation
        {
            get { return _orderCancellation; }
            set
            {
               _orderCancellation = value;
            }
        }
        
        public bool IsOrder
        {
            get { return _isOrder; }
            set { _isOrder = value; }
        }

        /// <summary>
        /// payload is either Order or CancelOrder
        /// </summary>
        /// <param name="payload"></param>
        /// <returns></returns>
        public static InputPayload CreatePayload(object payload)
        {
            InputPayload inputPayload = null;
            if (payload is Order)
            {
                inputPayload=new InputPayload();
                inputPayload.Order = payload as Order;
                inputPayload.IsOrder = true;
            }
            else if(payload is OrderCancellation)
            {
                inputPayload=new InputPayload();
                inputPayload.OrderCancellation = payload as OrderCancellation;
                inputPayload.IsOrder = false;
            }
            return inputPayload;
        }
    }
}

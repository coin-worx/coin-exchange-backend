using CoinExchange.Trades.Domain.Model.Order;

namespace CoinExchange.Trades.Application.OrderServices.Representation
{
    /// <summary>
    /// New order created representation when a new order is placed
    /// </summary>
    public class NewOrderRepresentation
    {
        public NewOrderRepresentation(decimal price, string type, string side, string pair, string orderId,decimal volume)
        {
            Price = price;
            Type = type;
            Side = side;
            Pair = pair;
            OrderId = orderId;
            Volume = volume;
        }

        /// <summary>
        /// create order representation from order
        /// </summary>
        /// <param name="order"></param>
        public NewOrderRepresentation(Order order)
        {
            Type = order.OrderType.ToString();
            Side = order.OrderSide.ToString();
            Pair = order.CurrencyPair;
            OrderId = order.OrderId.Id.ToString();
            Volume = order.Volume.Value;
            if (order.OrderType == OrderType.Limit)
            {
                Price = order.Price.Value;
            }
        }

        public string OrderId { get; private set; }
        public string Pair { get; private set; }
        public string Side { get; private set; }
        public string Type { get; private set; }
        public decimal Price { get; private set; }
        public decimal Volume { get; set; }

    }
}

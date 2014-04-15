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

        public string OrderId { get; private set; }
        public string Pair { get; private set; }
        public string Side { get; private set; }
        public string Type { get; private set; }
        public decimal Price { get; private set; }
        public decimal Volume { get; set; }

    }
}

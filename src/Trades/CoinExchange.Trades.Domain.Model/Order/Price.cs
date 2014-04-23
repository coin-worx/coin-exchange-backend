using CoinExchange.Common.Domain.Model;

namespace CoinExchange.Trades.Domain.Model.Order
{
    /// <summary>
    /// order limit price value object
    /// </summary>
    public class Price
    {
        private decimal _value;
        public decimal Value
        {
            get { return _value; }
            private set
            {
                _value = value;
            }
        }

        public Price(decimal value)
        {
            Value = value;
        }

        public bool IsGreaterThan(Price price)
        {
            if (price == null)
            {
                return false;
            }
            return _value > price._value;
        }

        public override string ToString()
        {
            return "Order Limit Price=" + _value;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            return this._value == (obj as Price)._value;
        }
    }
}

using CoinExchange.Common.Domain.Model;

namespace CoinExchange.Trades.Domain.Model.Order
{
    /// <summary>
    /// order volume value object
    /// </summary>
    public class Volume
    {
        private decimal _value;
        public decimal Value
        {
            get { return _value; }
            private set
            {
                AssertionConcern.AssertGreaterThanZero(value,"Volume must be greater than 0");
                _value = value;
            }
        }

        public Volume(decimal value)
        {
            Value = value;
        }

        public bool IsGreaterThan(Volume volume)
        {
            if (volume == null)
            {
                return false;
            }
            return _value > volume._value;
        }

        public override string ToString()
        {
            return "Order Volume=" + _value;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            return this._value == (obj as Volume)._value;
        }
    }
}

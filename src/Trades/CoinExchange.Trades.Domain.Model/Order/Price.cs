using System;
using CoinExchange.Common.Domain.Model;

namespace CoinExchange.Trades.Domain.Model.Order
{
    /// <summary>
    /// order limit price value object
    /// </summary>
    public class Price : IComparable<Price>
    {
        private decimal _value;
        public decimal Value
        {
            get { return _value; }
            private set
            {
                AssertionConcern.AssertGreaterThanZero(value,"Limit price must be greater than 0");
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

        public int CompareTo(Price price)
        {
            if (this.Value > price.Value)
            {
                return -1;
            }
            if (this.Value == price.Value)
            {
                return 0;
            }
            return 1;
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

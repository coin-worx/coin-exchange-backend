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

        public bool IsLessThan(Price price)
        {
            if (price == null)
            {
                return false;
            }
            return _value < price._value;
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

        /// <summary>
        /// += operator
        /// </summary>
        /// <param name="price1"></param>
        /// <param name="price2"></param>
        /// <returns></returns>
        public static Price operator +(Price price1, Price price2)
        {
            if (price1 == null || price2 == null)
            {
                return null;
            }
            return new Price(price1.Value + price2.Value);
        }

        /// <summary>
        /// += operator
        /// </summary>
        /// <param name="price"></param>
        /// <param name="price2"></param>
        /// <returns></returns>
        public static Price operator -(Price price, Price price2)
        {
            if (price == null || price2 == null)
            {
                return null;
            }
            return new Price(price.Value - price2.Value);
        }

        public static bool operator >(Price x, Price y)
        {
            return x.Value > y.Value;
        }

        public static bool operator <(Price x, Price y)
        {
            return x.Value < y.Value;
        }

        public static bool operator >=(Price x, Price y)
        {
            return x.Value >= y.Value;
        }

        public static bool operator <=(Price x, Price y)
        {
            return x.Value <= y.Value;
        }
    }
}

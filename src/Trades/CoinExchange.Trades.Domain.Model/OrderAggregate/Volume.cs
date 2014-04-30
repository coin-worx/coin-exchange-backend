using System;
using CoinExchange.Common.Domain.Model;

namespace CoinExchange.Trades.Domain.Model.OrderAggregate
{
    /// <summary>
    /// order volume value object
    /// </summary>
    [Serializable]
    public class Volume : IComparable<Volume>
    {
        private decimal _value;
        public decimal Value
        {
            get { return _value; }
            private set
            {
                //AssertionConcern.AssertGreaterThanZero(value,"Volume must be greater than 0");
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

        public int CompareTo(Volume volume)
        {
            if (this.Value > volume.Value) return -1;
            if (this.Value == volume.Value) return 0;
            return 1;
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

        /// <summary>
        /// += operator
        /// </summary>
        /// <param name="volume1"></param>
        /// <param name="volume2"></param>
        /// <returns></returns>
        public static Volume operator +(Volume volume1, Volume volume2)
        {
            if (volume1 == null || volume2 == null)
            {
                return null;
            }
            return new Volume(volume1.Value + volume2.Value);
        }

        /// <summary>
        /// += operator
        /// </summary>
        /// <param name="volume1"></param>
        /// <param name="volume2"></param>
        /// <returns></returns>
        public static Volume operator -(Volume volume1, Volume volume2)
        {
            if (volume1 == null || volume2 == null)
            {
                return null;
            }
            return new Volume(volume1.Value - volume2.Value);
        }

        public static bool operator >(Volume x, Volume y)
        {
            return x.Value > y.Value;
        }

        public static bool operator <(Volume x, Volume y)
        {
            return x.Value < y.Value;
        }

        public static bool operator >=(Volume x, Volume y)
        {
            return x.Value >= y.Value;
        }

        public static bool operator <=(Volume x, Volume y)
        {
            return x.Value <= y.Value;
        }
    }
}

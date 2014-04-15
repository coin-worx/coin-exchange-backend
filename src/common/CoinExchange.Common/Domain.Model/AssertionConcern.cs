using System;
using System.Text.RegularExpressions;

namespace CoinExchange.Common.Domain.Model
{
    /// <summary>
    /// serves for the purpose of validating objects and inputs
    /// </summary>
    public static class AssertionConcern
    {
        public static void AssertArgumentNotNull(object object1, string message)
        {
            if (object1 == null)
            {
                throw new InvalidOperationException(message);
            }
        }

        public static void AssertArgumentRange(decimal value, decimal minimum, decimal maximum, string message)
        {
            if (value < minimum || value > maximum)
            {
                throw new InvalidOperationException(message);
            }
        }

        public static void AssertEmptyString(string value, string message)
        {
            if (value == null || value.Equals(string.Empty))
            {
                throw new InvalidOperationException(message);
            }
        }

        public static void AssertGreaterThanZero(decimal value, string message)
        {
            if (value<=0)
            {
                throw new InvalidOperationException(message);
            }
        }
    }
}

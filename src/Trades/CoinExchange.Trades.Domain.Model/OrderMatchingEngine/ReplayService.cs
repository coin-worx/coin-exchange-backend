using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinExchange.Trades.Domain.Model.OrderMatchingEngine
{
    /// <summary>
    /// replay service only exchange is allowed to alter it remaining classes can only access it
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ReplayService
    {
        public static bool ReplayMode { get; private set; }

        /// <summary>
        /// Turn replay mode on
        /// </summary>
        public static void TurnReplayModeOn<T>(T type)
        {
            if (type is Exchange)
            {
                ReplayMode = true;
                return;
            }
            throw new InvalidOperationException("Operation not allowed to the callee");
        }

        /// <summary>
        /// turn replay mode off
        /// </summary>
        public static void TurnReplayModeOff<T>(T type)
        {
            if (type is Exchange)
            {
                ReplayMode = true;
                return;
            }
            throw new InvalidOperationException("Operation not allowed to the callee");
        }
    }
}

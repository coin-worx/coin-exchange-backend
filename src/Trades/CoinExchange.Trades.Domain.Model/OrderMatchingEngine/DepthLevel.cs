using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Trades.Domain.Model.Order;

namespace CoinExchange.Trades.Domain.Model.OrderMatchingEngine
{
    /// <summary>
    /// Contains aggregated volume and the number of Orders present at a depth level(price in market)
    /// </summary>
    public class DepthLevel
    {
        // Get the Current Logger
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger
        (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private Price _price;
        private Volume _aggregatedVolume;
        private int _orderCount = 0;
        private bool _isEmpty = false;

        /// <summary>
        /// Default Constructor
        /// </summary>
        public DepthLevel(Price price)
        {
            _price = price;
        }

        #region Methods

        /// <summary>
        /// Add the Order quantity to this depth level
        /// </summary>
        /// <returns></returns>
        public bool AddOrder(Volume volume)
        {
            if (volume != null)
            {
                if (_aggregatedVolume == null)
                {
                    _aggregatedVolume = volume;
                }
                else
                {
                    _aggregatedVolume += volume;
                }
                _orderCount++;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Remove one order from Order Count, eliminate the given quantity, remove order count if no orders remain
        /// </summary>
        /// <returns></returns>
        public bool CloseOrder(Volume volume)
        {
            if (_orderCount == 0)
            {
                Log.Error("No order remaining for Price level: " + _price.Value);
            }
            else if (_orderCount == 1)
            {
                _orderCount = 0;
                _aggregatedVolume = new Volume(0);
                _isEmpty = true;
                return true;
            }
            else
            {
                _orderCount--;
                if (_aggregatedVolume.Value >= volume.Value)
                {
                    _aggregatedVolume -= volume;
                    return true;
                }
                else
                {
                    Log.Error("Requested volume to remove: " + volume.Value + " is greater than the current volume: " + 
                        _aggregatedVolume.Value);
                }
            }
            return false;
        }

        /// <summary>
        /// Increase Volume
        /// </summary>
        /// <param name="volume"></param>
        /// <returns></returns>
        public bool IncreaseVolume(Volume volume)
        {
            if (volume != null)
            {
                _aggregatedVolume += volume;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Increase Volume
        /// </summary>
        /// <param name="volume"></param>
        /// <returns></returns>
        public bool DecreaseVolume(Volume volume)
        {
            if (volume != null)
            {
                _aggregatedVolume -= volume;
                return true;
            }
            return false;
        }

        #endregion Methods

        #region Properties

        /// <summary>
        /// The price at which this depth level is located
        /// </summary>
        public Price Price
        {
            get
            {
                return _price;
            }
        }

        /// <summary>
        /// The Aggregated volume of the orders a t this depth level
        /// </summary>
        public Volume AggregatedVolume
        {
            get
            {
                return _aggregatedVolume;
            }
        }

        /// <summary>
        /// The number of Orders located at this price level
        /// </summary>
        public int OrderCount
        {
            get
            {
                return _orderCount;
            }
        }

        /// <summary>
        /// Is the Depth Level empty
        /// </summary>
        public bool IsEmpty
        {
            get
            {
                return _isEmpty;
            }
            set
            {
                _isEmpty = value;
            }
        }

        #endregion Properties
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Trades.Domain.Model.OrderAggregate;

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

        private ChangeId _changeId;
        private Price _price;
        private Volume _aggregatedVolume;
        private int _orderCount = 0;
        private bool _isEmpty = false;
        private bool _isExcess = false;

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
        /// Remove one order from Order Count, eliminate the given quantity, return true if no orders remain
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
                _aggregatedVolume -= volume;
                _isEmpty = true;
                return true;
            }
            else
            {
                decimal remainingVolume = _aggregatedVolume.Value - volume.Value;
                // If there is no volume remaining but still one or more orders are remaining, than this operation should not
                // be allowed
                if (remainingVolume != 0)
                {
                    _orderCount--;
                    if (_aggregatedVolume.Value >= volume.Value)
                    {
                        _aggregatedVolume -= volume;
                    }
                    else
                    {
                        Log.Error("Requested volume to remove: " + volume.Value +
                                  " is greater than the current volume: " +
                                  _aggregatedVolume.Value);
                    }
                }
                else
                {
                    throw new InvalidOperationException("One or more orders are remaining but the volume specified eliminates " +
                                                        "all the volume, so operation cannot be commenced.");
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
        /// Update the price, this will be used when replacing orders and levels movement between depth level slots
        /// </summary>
        /// <param name="price"></param>
        /// <returns></returns>
        public bool UpdatePrice(Price price)
        {
            if (price != null)
            {
                // Done this way so the reference of one level's object does not get copied to another level, otherwise both
                // levels will get affected due to a change in one
                _price = new Price(price.Value);
            }
            else
            {
                // For levels that are removed, only values will become null, but levels will stay on that slot physically
                _price = null;
            }
            return true;
        }

        /// <summary>
        /// Adds volume to this Depth level, this will be used while moving price levels between slots in depths
        /// </summary>
        /// <returns></returns>
        public bool UpdateVolume(Volume volume)
        {
            if (volume != null)
            {
                // Done this way so the reference of one level's object does not get copied to another level, otherwise both
                // levels will get affected due to a change in one
                _aggregatedVolume = new Volume(volume.Value);
            }
            else
            {
                _aggregatedVolume = null;
            }
            return true;
        }

        /// <summary>
        /// Adds the Order count to this level, this will be done when moving levels between slots in the depth
        /// </summary>
        /// <param name="orderCount"></param>
        /// <returns></returns>
        public bool UpdateOrderCount(int orderCount)
        {
            _orderCount = orderCount;
            return true;
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

        /// <summary>
        /// Adds Order Count. This will be used when we will move depth levels
        /// </summary>
        /// <returns></returns>
        public bool AddOrderCount()
        {
            _orderCount++;
            return true;
        }

        /// <summary>
        /// Last change that occured to this level
        /// </summary>
        /// <param name="changeId"></param>
        public void LastChange(ChangeId changeId)
        {
            _changeId = changeId;
        }

        /// <summary>
        /// Changes the status that describes whether the level is an excess level or not
        /// </summary>
        /// <param name="isExcess"></param>
        public void ChangeExcessStatus(bool isExcess)
        {
            _isExcess = isExcess;
        }

        /// <summary>
        /// Specifies whether this level has changed since the last published change provided by the caller
        /// </summary>
        /// <returns></returns>
        public bool ChangedSince(decimal lastPublishedChange)
        {
            return _changeId.Id > lastPublishedChange;
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
        /// The price at which this depth level is located
        /// </summary>
        public ChangeId ChangeId
        {
            get
            {
                return _changeId;
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

        /// <summary>
        /// Is this an Excess level? Outside of the levels that we show to the clients?
        /// </summary>
        public bool IsExcess
        {
            get
            {
                return _isExcess;
            }
        }

        #endregion Properties

        #region Operator overrides

        public static bool operator >(DepthLevel x, DepthLevel y)
        {
            return x.Price > y.Price;
        }

        public static bool operator <(DepthLevel x, DepthLevel y)
        {
            return x.Price < y.Price;
        }

        public static bool operator >=(DepthLevel x, DepthLevel y)
        {
            return x.Price >= y.Price;
        }

        public static bool operator <=(DepthLevel x, DepthLevel y)
        {
            return x.Price <= y.Price;
        }

        #endregion Operators overrides
    }
}

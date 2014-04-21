using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Trades.Domain.Model.Order;

namespace CoinExchange.Trades.Domain.Model.OrderMatchingEngine
{
    /// <summary>
    /// Contains the Depth levels for each price in the market
    /// </summary>
    public class Depth
    {
        #region Private fields

        private string _currencyPair = string.Empty;
        private int _size = 0;
        private DepthLevel _bestBid = null;
        private DepthLevel _bestAsk = null;
        private DepthLevel _lastBid = null;
        private DepthLevel _lastAsk = null;

        private DepthLevel[] _bidLevels = null;
        private DepthLevel[] _askLevels = null;

        // Gets one past the last ask level
        private DepthLevel _end = null;

        private int _lastChangeId = 0;
        private int _lastPublishedChangeId = 0;

        private Volume _ignoreBidFillVolume = null;
        private Volume _ignoreAskFillVolume = null;

        #endregion Private Fields

        /// <summary>
        /// Default constructor
        /// </summary>
        public Depth(string currencyPair, int size)
        {
            _currencyPair = currencyPair;
            _size = size;

            _bidLevels = new DepthLevel[size];
            _askLevels = new DepthLevel[size];

            for (int i = 0; i < _bidLevels.Length; i++)
            {
                _bidLevels[i] = new DepthLevel(null);
                _askLevels[i] = new DepthLevel(null);
            }

            _bestBid = _bidLevels[0];
            _bestAsk = _askLevels[0];
            _lastBid = _bidLevels.Last();
            _lastAsk = _askLevels.Last();
        }

        #region Methods

        /// <summary>
        /// Add Order to the Depth
        /// </summary>
        /// <returns></returns>
        public void AddOrder(Price price, Volume volume, OrderSide orderSide)
        {
            int lastChangeIdCopy = _lastChangeId;
            DepthLevel depthLevel = null;
            switch (orderSide)
            {
                    case OrderSide.Buy:
                    depthLevel = FindLevel(price, orderSide, _bidLevels);
                    break;
                    case OrderSide.Sell:
                    depthLevel = FindLevel(price, orderSide, _askLevels);
                    break;
            }
            if (depthLevel != null)
            {
                depthLevel.AddOrder(volume);

                // Note: As there are no excess levels, we will mark change in every depth with a change Id. We won't mark
                // a change if it is not a visible level,but in our case we don't have any excess levels so every level
                // is as visible level
                _lastChangeId = lastChangeIdCopy + 1;
                depthLevel.LastChange(new ChangeId(lastChangeIdCopy + 1));
            }
        }

        /// <summary>
        /// Ignore future fill quantity on a side, due to a match at accept time for an order
        /// </summary>
        /// <param name="price"></param>
        /// <param name="volume"> </param>
        /// <param name="orderSide"></param>
        public void IgnoreFillQuantity(Price price, Volume volume, OrderSide orderSide)
        {
            switch (orderSide)
            {
                    case OrderSide.Sell:
                    _ignoreAskFillVolume = volume;
                    break;

                    case OrderSide.Buy:
                    _ignoreBidFillVolume = volume;
                    break;
            }
        }

        /// <summary>
        /// Handle an order fill
        /// </summary>
        /// <param name="price"></param>
        /// <param name="qty"></param>
        /// <param name="filled"></param>
        /// <param name="orderSide"></param>
        public void FillOrder(Price price, Volume qty,bool filled, OrderSide orderSide)
        {
            if (orderSide == OrderSide.Buy && _ignoreBidFillVolume.Value != 0)
            {

            }
            else if (orderSide == OrderSide.Sell && _ignoreAskFillVolume.Value != 0)
            {

            }
            else if (filled)
            {
                CloseOrder(price, qty, orderSide);
            }
            else
            {
                ChangeOrderQuantity(price, -qty.Value, orderSide);
            }
        }

        /// <summary>
        /// Cancel or fill an order
        /// </summary>
        /// <param name="price"></param>
        /// <param name="volume"></param>
        /// <param name="orderSide"> </param>
        /// <returns></returns>
        public bool CloseOrder(Price price, Volume volume, OrderSide orderSide)
        {
            DepthLevel level = FindLevel(price, orderSide, orderSide == OrderSide.Sell ? _askLevels : _bidLevels);
            if (level != null)
            {
                // If no levels remain
                if (level.CloseOrder(volume))
                {
                    EraseLevel(level, orderSide);
                }
                // Else, mark the level as changed
                else
                {
                    level.LastChange(new ChangeId(++_lastChangeId));
                }
            }
            return false;
        }

        /// <summary>
        /// Change quantity of an order
        /// </summary>
        /// <param name="price"></param>
        /// <param name="quantity"></param>
        /// <param name="orderSide"></param>
        public void ChangeOrderQuantity(Price price, decimal quantity, OrderSide orderSide)
        {
            DepthLevel depthLevel = FindLevel(price, orderSide, orderSide == OrderSide.Buy ? _bidLevels : _askLevels);
            if (depthLevel != null && quantity != 0)
            {
                // If volume is positive, increase quantity(used when order is replaced)
                if (quantity > 0)
                {
                    depthLevel.IncreaseVolume(new Volume(quantity));
                }
                // If volume is negative, decrease the quantity(used when orders fill or get replaced)
                else
                {
                    // If after decreasing the volume, the volume becomes 0, then we will need to remove this level from depth
                    if (depthLevel.AggregatedVolume.Value - (-quantity) == 0)
                    {
                        EraseLevel(depthLevel, orderSide);
                    }
                    else
                    {
                        // We will convert the negative quantity into positive as Volume does not instantiate with a negative
                        // quantity, and also we are already decreasing the volume from the depth level so negative value is 
                        // not required
                        depthLevel.DecreaseVolume(new Volume(-quantity));
                    }
                }
                depthLevel.LastChange(new ChangeId(_lastChangeId++));
            }
        }

        /// <summary>
        /// Replace an Order
        /// </summary>
        /// <param name="currentPrice"></param>
        /// <param name="newPrice"></param>
        /// <param name="currentQuantity"></param>
        /// <param name="newQuantity"></param>
        /// <param name="orderSide"></param>
        public void ReplaceOrder(Price currentPrice, Price newPrice, Volume currentQuantity, Volume newQuantity,
                                  OrderSide orderSide)
        {

        }

        /// <summary>
        /// Does this depth need bid restoration after level erasure
        /// </summary>
        /// <param name="restorationPrice">The price to restore after (out)</param>
        /// <returns>True if restoration is needed (previously was full)</returns>
        public bool NeedsBidRestoration(Price restorationPrice)
        {
            return false;
        }

        /// <summary>
        /// Does this depth need ask restoration after level erasure
        /// </summary>
        /// <param name="restorationPrice">The price to restore after (out)</param>
        /// <returns> true if restoration is needed (previously was full)</returns>
        public bool NeedsAskRestoration(Price restorationPrice)
        {
            return false;
        }

        /// <summary>
        /// Has the depth changed since the last publish
        /// </summary>
        /// <returns></returns>
        public bool Changed()
        {
            return false;
        }

        /// <summary>
        /// note the ID of last published change
        /// </summary>
        public bool Published()
        {
            return _lastChangeId > _lastPublishedChangeId;
        }

        #region Level Finders

        /// <summary>
        /// Find the level specified by price, create new if does not exist
        /// </summary>
        public DepthLevel FindLevel(Price price, OrderSide orderSide, DepthLevel[] depthLevels)
        {
            // No excess levels being supported, because the default allocation for depths is a large one. Can provide excess
            // levels based on requirements
            foreach (var depthLevel in depthLevels)
            {
                // If the price is null in this depth level, initialize this depth level with the current price
                if (depthLevel.Price == null)
                {
                    depthLevel.Price = price;
                    return depthLevel;
                }
                else if(depthLevel.Price.Equals(price))
                {
                    return depthLevel;
                }
                // If the order side is buy and the inbound price is greater than the price present at the current depth level
                // being analyzed, insert the new price in this slot and move the current level one level down
                else if (orderSide == OrderSide.Buy && price.IsGreaterThan(depthLevel.Price))
                {
                    return InsertLevelBefore(depthLevel, price, _lastBid, depthLevels);
                }
                // If the order side is sell and the inbound price is less than the price present at the current depth level 
                // being analyzed, insert the new price in this slot and move the current level one level down
                else if (orderSide == OrderSide.Sell && price.IsLessThan(depthLevel.Price))
                {
                    return InsertLevelBefore(depthLevel, price, _lastAsk, depthLevels);
                }
            }
            return null;
        }

        /// <summary>
        /// Insert the Order before a certain level
        /// </summary>
        /// <param name="lcurrentLevel"></param>
        /// <param name="price"></param>
        /// <param name="lastSideLevel"> </param>
        /// <param name="sideLevels"> </param>
        private DepthLevel InsertLevelBefore(DepthLevel lcurrentLevel, Price price, DepthLevel lastSideLevel,
            DepthLevel[] sideLevels)
        {
            // Note: Can provide a a separate place for excess level bids and asks, and figure a price level from that
            // if the price goes above or below than that price, then we need to store the order in the excess levels
            // Currently, we will only support a high allocation of the bid and ask levels and no excess levels

            int currentLevelIndex = Array.FindIndex(sideLevels, depthLevel => depthLevel == lcurrentLevel);
            int backEndLevelIndex = Array.FindIndex(sideLevels, depthLevel => depthLevel == lastSideLevel) - 1;

            ++_lastChangeId;

            while (backEndLevelIndex >= currentLevelIndex)
            {
                if (sideLevels[backEndLevelIndex].Price != null)
                {
                    sideLevels[backEndLevelIndex + 1] =
                        new DepthLevel(new Price(sideLevels[backEndLevelIndex].Price.Value))
                            {
                                IsEmpty = sideLevels[backEndLevelIndex].IsEmpty
                            };
                    UpdateIndex(sideLevels, backEndLevelIndex + 1, backEndLevelIndex);
                    /*if (sideLevels[backEndLevelIndex].AggregatedVolume != null)
                    {
                        sideLevels[backEndLevelIndex + 1].AggregatedVolume = sideLevels[backEndLevelIndex].AggregatedVolume;
                    }
                    for (int i = 0; i < sideLevels[backEndLevelIndex].OrderCount; i++)
                    {
                        sideLevels[backEndLevelIndex + 1].AddOrderCount();
                    }*/
                }
                if (sideLevels[backEndLevelIndex].Price != null)
                {
                    sideLevels[backEndLevelIndex + 1].LastChange(new ChangeId(_lastChangeId));
                }
                backEndLevelIndex--;
            }
            sideLevels[currentLevelIndex] = new DepthLevel(price);
            return sideLevels[currentLevelIndex];
        }

        /// <summary>
        /// Erase the specified level from the Depth Levels
        /// </summary>
        /// <param name="inboundLevel"> </param>
        /// <param name="orderSide"></param>
        /// <returns></returns>
        public void EraseLevel(DepthLevel inboundLevel, OrderSide orderSide)
        {
            DepthLevel lastSideLevel = orderSide == OrderSide.Buy ? _lastBid : _lastAsk;
            DepthLevel[] sideLevels = orderSide == OrderSide.Buy ? _bidLevels : _askLevels;

            int backEndLevelIndex = Array.FindIndex(sideLevels, depthLevel => depthLevel == lastSideLevel);
            int inboundLevelIndex = Array.FindIndex(sideLevels, depthLevel => depthLevel == inboundLevel);
            int currentLevelIndex = inboundLevelIndex;

            ++_lastChangeId;
            
            while (currentLevelIndex < backEndLevelIndex)
            {
                if (currentLevelIndex == inboundLevelIndex)
                {
                    sideLevels[currentLevelIndex] = new DepthLevel(null);
                }
                sideLevels[currentLevelIndex] = new DepthLevel(sideLevels[currentLevelIndex + 1].Price);
                UpdateIndex(sideLevels, currentLevelIndex, currentLevelIndex + 1);

                currentLevelIndex++;
            }
        }

        /// <summary>
        /// Updates the volume and order count for a specific index copied from another index
        /// </summary>
        /// <returns></returns>
        public bool UpdateIndex(DepthLevel[] sideLevels, int sourceIndex, int targetIndex)
        {
            try
            {
                if (sideLevels != null)
                {
                    if (sideLevels[targetIndex].AggregatedVolume != null)
                    {
                        sideLevels[sourceIndex].AddVolume(sideLevels[targetIndex].AggregatedVolume);
                    }
                    sideLevels[sourceIndex].AddOrderCount(sideLevels[targetIndex].OrderCount);
                    return true;
                }
            }
            catch (Exception)
            {
                throw new InvalidOperationException();
            }
            return false;
        }

        private bool IsValidPrice(Price price)
        {
            // ToDo: Can provide a constraint for the price to be a valid price
            return true;
        }

        #endregion Level Finders

        #endregion Methods

        #region Properties

        /// <summary>
        /// CurrencyPair
        /// </summary>
        public string CurrencyPair
        {
            get
            {
                return _currencyPair;
            }
        }

        /// <summary>
        /// BidLevels
        /// </summary>
        public DepthLevel[] BidLevels
        {
            get
            {
                return _bidLevels;
            }
        }

        /// <summary>
        /// AskLevels
        /// </summary>
        public DepthLevel[] AskLevels
        {
            get
            {
                return _askLevels;
            }
        }

        /// <summary>
        /// Last Change's Id
        /// </summary>
        public int LastChangeId
        {
            get
            {
                return _lastChangeId;
            }
        }

        /// <summary>
        /// Last Published Change's Id
        /// </summary>
        public int LastPublishedChangeId
        {
            get
            {
                return _lastPublishedChangeId;
            }
        }

        /// <summary>
        /// The best bid of the book
        /// </summary>
        public DepthLevel BestBid
        {
            get
            {
                return _bestBid;
            }
        }

        /// <summary>
        /// The best ask of the book
        /// </summary>
        public DepthLevel BestAsk
        {
            get
            {
                return _bestAsk;
            }
        }

        /// <summary>
        /// The last bid of the book
        /// </summary>
        public DepthLevel LastBid
        {
            get
            {
                return _lastBid;
            }
        }

        /// <summary>
        /// The last ask of the book
        /// </summary>
        public DepthLevel LastAsk
        {
            get
            {
                return _lastAsk;
            }
        }

        #endregion Properties
    }
}

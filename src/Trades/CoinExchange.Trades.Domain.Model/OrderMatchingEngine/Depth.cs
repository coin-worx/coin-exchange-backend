using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Trades.Domain.Model.OrderAggregate;

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

        private DepthLevelMap _bidExcessLevels = null;
        private DepthLevelMap _askExcessLevels = null;

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

            _bidExcessLevels = new DepthLevelMap(currencyPair, OrderSide.Buy);
            _askExcessLevels = new DepthLevelMap(currencyPair, OrderSide.Sell);
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
        /// <param name="volume"> </param>
        /// <param name="orderSide"></param>
        public void IgnoreFillQuantity(Volume volume, OrderSide orderSide)
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
        /// <param name="originalOrderPrice">The original price of the order on which a level is created in the depth</param>
        /// <param name="filledPrice"></param>
        /// <param name="filledVolume"></param>
        /// <param name="filled"></param>
        /// <param name="orderSide"></param>
        public void FillOrder(Price originalOrderPrice, Price filledPrice, Volume filledVolume, bool filled, OrderSide orderSide)
        {
            if (orderSide == OrderSide.Buy && _ignoreBidFillVolume != null)
            {
                _ignoreBidFillVolume -= filledVolume;
            }
            else if (orderSide == OrderSide.Sell && _ignoreAskFillVolume != null)
            {
                _ignoreAskFillVolume -= filledVolume;
            }
            else if (filled)
            {
                CloseOrder(originalOrderPrice, filledVolume, orderSide);
            }
            else
            {
                ChangeOrderQuantity(originalOrderPrice, -filledVolume.Value, orderSide);
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
        public bool NeedsBidRestoration(out Price restorationPrice)
        {
            if (_size > 1)
            {
                // Check if the second last level of the bids is not null, restore using that level
                if (_bidLevels[Array.FindIndex(_bidLevels, depthLevel => depthLevel == _bidLevels.Last()) - 1].Price.Value != 0)
                {
                    restorationPrice = _bidLevels[Array.FindIndex(_bidLevels, depthLevel => depthLevel == _bidLevels.Last()) - 1].Price;
                    return true;
                }
            }
            // Otherwise this depth is BBO only
            else if(_size == 1)
            {
                restorationPrice = new Price(ConstantTypes.MARKET_ORDER_BID_SORT_PRICE);
                return true;
            }
            restorationPrice = null;
            return false;
        }

        /// <summary>
        /// Does this depth need ask restoration after level erasure
        /// </summary>
        /// <param name="restorationPrice">The price to restore after (out)</param>
        /// <returns> true if restoration is needed (previously was full)</returns>
        public bool NeedsAskRestoration(out Price restorationPrice)
        {
            if (_size > 1)
            {
                // Check if the second last level of the asks is not null, restore using that level
                if (_askLevels[Array.FindIndex(_askLevels, depthLevel => depthLevel == _askLevels.Last()) - 1].Price.Value != 0)
                {
                    restorationPrice = _askLevels[Array.FindIndex(_askLevels, depthLevel => depthLevel == _askLevels.Last()) - 1].Price;
                    return true;
                }
            }
            // Otherwise this depth is BBO only
            else if (_size == 0)
            {
                restorationPrice = new Price(ConstantTypes.MARKET_ORDER_ASK_SORT_PRICE);
                return true;
            }
            restorationPrice = null;
            return false;
        }

        /// <summary>
        /// Has the depth changed since the last publish
        /// </summary>
        /// <returns></returns>
        public bool Changed()
        {
            return _lastChangeId > _lastPublishedChangeId;
        }

        /// <summary>
        /// note the ID of last published change
        /// </summary>
        public void Published()
        {
            _lastPublishedChangeId = _lastChangeId;
        }

        #region Level Finders

        /// <summary>
        /// Find the level specified by price, create new if does not exist
        /// </summary>
        public DepthLevel FindLevel(Price price, OrderSide orderSide, DepthLevel[] depthLevels)
        {
            DepthLevel foundDepthLevel = null;
            // No excess levels being supported, because the default allocation for depths is a large one. Can provide excess
            // levels based on requirements
            foreach (var depthLevel in depthLevels)
            {
                // If the price is null in this depth level, initialize this depth level with the current price
                if (depthLevel.Price == null || depthLevel.Price.Value == 0)
                {
                    depthLevel.UpdatePrice(price);
                    foundDepthLevel = depthLevel;
                    break;
                }
                else if(depthLevel.Price.Equals(price))
                {
                    foundDepthLevel = depthLevel;
                    break;
                }
                // If the order side is buy and the inbound price is greater than the price present at the current depth level
                // being analyzed, insert the new price in this slot and move the current level one level down
                else if (orderSide == OrderSide.Buy && price.IsGreaterThan(depthLevel.Price))
                {
                    foundDepthLevel = InsertLevelBefore(depthLevel, price, _bidLevels.Last(), depthLevels, orderSide);
                    break;
                }
                // If the order side is sell and the inbound price is less than the price present at the current depth level 
                // being analyzed, insert the new price in this slot and move the current level one level down
                else if (orderSide == OrderSide.Sell && price.IsLessThan(depthLevel.Price))
                {
                    foundDepthLevel = InsertLevelBefore(depthLevel, price, _askLevels.Last(), depthLevels, orderSide);
                    break;
                }
            }

            if (foundDepthLevel == null)
            {
                foundDepthLevel = FindInExcessLevels(price, orderSide);
                if (foundDepthLevel == null)
                {
                    foundDepthLevel = new DepthLevel(price);
                    if (orderSide == OrderSide.Buy)
                    {
                        _bidExcessLevels.AddLevel(price, foundDepthLevel);
                    }
                    else
                    {
                        _askExcessLevels.AddLevel(price, foundDepthLevel);
                    }
                }
            }
            return foundDepthLevel;
        }

        /// <summary>
        /// Insert the Order before a certain level
        /// </summary>
        /// <param name="currentLevel"></param>
        /// <param name="price"></param>
        /// <param name="lastSideLevel"> </param>
        /// <param name="sideLevels"> </param>
        /// <param name="orderSide"> </param>
        private DepthLevel InsertLevelBefore(DepthLevel currentLevel, Price price, DepthLevel lastSideLevel,
            DepthLevel[] sideLevels, OrderSide orderSide)
        {
            if (sideLevels.Last().Price != null && sideLevels.Last().Price.Value != 0)
            {
                DepthLevel depthLevel = InitiateExcessValues(sideLevels.Last().Price, sideLevels.Last().AggregatedVolume, 
                    sideLevels.Last().OrderCount);

                switch (orderSide)
                {
                        case OrderSide.Buy:
                        _bidExcessLevels.AddLevel(depthLevel.Price, depthLevel);
                        break;

                        case OrderSide.Sell:
                        _askExcessLevels.AddLevel(depthLevel.Price, depthLevel);
                        break;
                }
            }

            int currentLevelIndex = Array.FindIndex(sideLevels, depthLevel => depthLevel == currentLevel);
            int backEndLevelIndex = Array.FindIndex(sideLevels, depthLevel => depthLevel == lastSideLevel) - 1;

            ++_lastChangeId;

            // Move from the lowest levels, move every slot one place ahead so that the new depth level can be inserted
            while (backEndLevelIndex >= currentLevelIndex)
            {
                if (sideLevels[backEndLevelIndex].Price != null)
                {
                    sideLevels[backEndLevelIndex + 1] = sideLevels[backEndLevelIndex];
                    UpdateLevelIndex(sideLevels, backEndLevelIndex + 1, backEndLevelIndex);
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
        /// Find the required level in the Excess Level mantainers for Bid ans Ask
        /// </summary>
        /// <returns></returns>
        private DepthLevel FindInExcessLevels(Price price, OrderSide orderSide)
        {
            switch (orderSide)
            {
                    case OrderSide.Buy:
                    foreach (var bidExcessLevel in _bidExcessLevels)
                    {
                        if (bidExcessLevel.Key == price.Value)
                        {
                            return bidExcessLevel.Value;
                        }
                    }

                    // If the level is not found and the above loop terminated without returning control from this function
                    return new DepthLevel(price);

                    case OrderSide.Sell:
                    foreach (var askExcessLevel in _askExcessLevels)
                    {
                        if (askExcessLevel.Key == price.Value)
                        {
                            return askExcessLevel.Value;
                        }
                    }
                    break;
            }
            return null;
        }

        /// <summary>
        /// Erase the specified level from the Depth Levels
        /// </summary>
        /// <param name="inboundLevel"> </param>
        /// <param name="orderSide"></param>
        /// <returns></returns>
        public void EraseLevel(DepthLevel inboundLevel, OrderSide orderSide)
        {
            // Specifies the last element with a non-null price in the current side Depthlevel array
            bool isLastLevel = orderSide == OrderSide.Buy ? (
                                                             // If the provided level is the last non-null price level
                                                             inboundLevel == FindLastLevel(_bidLevels) &&
                                                             // If the last non-null price level is the last slot in the array
                                                             FindLastLevel(_bidLevels) == _bidLevels.Last()) 
                                                             :
                                                             // If the provided level is the last non-null price level
                                                             (inboundLevel == FindLastLevel(_askLevels) &&
                                                             // If the last non-null price level is the last slot in the array
                                                             FindLastLevel(_askLevels) == _askLevels.Last());
            // If the level lies in the excess levels
            if (inboundLevel.IsExcess)
            {
                switch (orderSide)
                {
                    case OrderSide.Buy:
                        _bidExcessLevels.RemoveLevel(inboundLevel.Price);
                        break;

                    case OrderSide.Sell:
                        _askExcessLevels.RemoveLevel(inboundLevel.Price);
                        break;
                }
            }
            // Otherwise, remove the level from the current slot and move all other levels to one slot back
            else
            {
                DepthLevel lastSideLevel = orderSide == OrderSide.Buy ? _bidLevels.Last() : _askLevels.Last();
                DepthLevel[] sideLevels = orderSide == OrderSide.Buy ? _bidLevels : _askLevels;

                int backEndLevelIndex = Array.FindIndex(sideLevels, depthLevel => depthLevel == lastSideLevel);
                int inboundLevelIndex = Array.FindIndex(sideLevels, depthLevel => depthLevel == inboundLevel);
                int currentLevelIndex = inboundLevelIndex;

                ++_lastChangeId;

                // Move from the current level to the last
                while (currentLevelIndex < backEndLevelIndex)
                {
                    // In the first loop, currentLevelIndex starts from the level which is being removed, inboundLevelIndex
                    // so we make every value null
                    if (currentLevelIndex == inboundLevelIndex)
                    {
                        sideLevels[currentLevelIndex].UpdatePrice(new Price(0));
                        sideLevels[currentLevelIndex].UpdateVolume(new Volume(0));
                        sideLevels[currentLevelIndex].UpdateOrderCount(0);
                        sideLevels[currentLevelIndex].ChangeExcessStatus(false);
                    }
                    sideLevels[currentLevelIndex] = sideLevels[currentLevelIndex + 1];
                    UpdateLevelIndex(sideLevels, currentLevelIndex, currentLevelIndex + 1);
                    sideLevels[currentLevelIndex].LastChange(new ChangeId(_lastChangeId));

                    currentLevelIndex++;
                }
                // The last element after the loop is done will be the same as the second last one as the slots have been 
                // shifted back. So we remove this level as this is the duplicate of the level before it
                sideLevels[currentLevelIndex] = new DepthLevel(new Price(0));
                
                if (isLastLevel || sideLevels.Last().Price == null || sideLevels.Last().Price.Value == 0)
                {
                    if (orderSide == OrderSide.Buy && (isLastLevel || sideLevels.Last().Price == null || sideLevels.Last().Price.Value == 0))
                    {
                        RemoveLevelFromExcess(_bidExcessLevels, _bidLevels);
                    }
                    else if (orderSide == OrderSide.Sell && (isLastLevel || sideLevels.Last().Price == null || sideLevels.Last().Price.Value == 0))
                    {
                        RemoveLevelFromExcess(_askExcessLevels, _askLevels);
                    }
                    else
                    {
                        sideLevels[currentLevelIndex].LastChange(new ChangeId(_lastChangeId));
                    }
                }
                sideLevels[currentLevelIndex].LastChange(new ChangeId(_lastChangeId));
            }
        }

        /// <summary>
        /// Returns the last element with a non-null price in the current side Depthlevel array
        /// </summary>
        /// <returns></returns>
        public DepthLevel FindLastLevel(DepthLevel[] sideDepthLevel)
        {
            DepthLevel depthLevel = null;
            foreach (var currentDepthLevel in sideDepthLevel)
            {
                if (currentDepthLevel.Price != null)
                {
                    depthLevel = currentDepthLevel;
                }
                else
                {
                    break;
                }
            }
            return depthLevel;
        }

        /// <summary>
        /// removes the level from the specified side's excess level
        /// </summary>
        /// <returns></returns>
        private bool RemoveLevelFromExcess(DepthLevelMap depthLevelMap, DepthLevel[] sideLevels)
        {
            if (depthLevelMap.Any())
            {
                sideLevels.Last().UpdatePrice(depthLevelMap.First().Value.Price);
                sideLevels.Last().UpdateVolume(depthLevelMap.First().Value.AggregatedVolume);
                sideLevels.Last().UpdateOrderCount(depthLevelMap.First().Value.OrderCount);
                sideLevels.Last().ChangeExcessStatus(false);

                depthLevelMap.RemoveLevel(depthLevelMap.First().Value.Price);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Updates the volume and order count for a specific index copied from another index in the visible depth levels
        /// </summary>
        /// <returns></returns>
        private bool UpdateLevelIndex(DepthLevel[] sideLevels, int targetIndex, int sourceIndex)
        {
            try
            {
                if (sideLevels != null)
                {
                    if (sideLevels[sourceIndex].AggregatedVolume != null && sideLevels[sourceIndex].Price != null)
                    {
                        sideLevels[targetIndex].UpdatePrice(new Price(sideLevels[sourceIndex].Price.Value));
                        sideLevels[targetIndex].UpdateVolume(new Volume(sideLevels[sourceIndex].AggregatedVolume.Value));
                        sideLevels[targetIndex].UpdateOrderCount(sideLevels[sourceIndex].OrderCount);
                        sideLevels[targetIndex].ChangeExcessStatus(sideLevels[sourceIndex].IsExcess);
                        return true;
                    }
                }
            }
            catch (Exception)
            {
                throw new InvalidOperationException();
            }
            return false;
        }

        /// <summary>
        /// Updates the Depth Level for excess levels
        /// </summary>
        /// <param name="price"> </param>
        /// <param name="volume"></param>
        /// <param name="orderCount"></param>
        /// <returns></returns>
        private DepthLevel InitiateExcessValues(Price price, Volume volume, int orderCount)
        {
            try
            {
                DepthLevel depthLevel = null;

                if (price != null && volume != null)
                {
                    depthLevel = new DepthLevel(price);
                    depthLevel.UpdateVolume(volume);
                    depthLevel.UpdateOrderCount(orderCount);
                    depthLevel.ChangeExcessStatus(true);
                    return depthLevel;
                }
            }
            catch (Exception)
            {
                throw new InvalidOperationException();
            }
            return null;
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
        /// Bid Excess Levels
        /// </summary>
        public DepthLevelMap BidExcessLevels
        {
            get
            {
                return _bidExcessLevels;
            }
        }

        /// <summary>
        /// Ask Excess Levels
        /// </summary>
        public DepthLevelMap AskExcessLevels
        {
            get
            {
                return _askExcessLevels;
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

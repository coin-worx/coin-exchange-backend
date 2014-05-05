using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Trades.Domain.Model.OrderAggregate;
using CoinExchange.Trades.Domain.Model.TradeAggregate;

namespace CoinExchange.Trades.Domain.Model.OrderMatchingEngine
{
    /// <summary>
    /// Contains the depths for the price levels in the Order Book and updates as the LimitOrderBook updates
    /// </summary>
    [Serializable]
    public class DepthOrderBook : IOrderListener, IOrderBookListener
    {
        private string _currencyPair = string.Empty;
        private int _size = 0;
        private Depth _depth = null;

        public event Action<BBO> BboChanged;
        public event Action<Depth> DepthChanged;

        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="currencyPair"></param>
        /// <param name="size"> </param>
        public DepthOrderBook(string currencyPair, int size)
        {
            _currencyPair = currencyPair;
            _size = size;
            _depth = new Depth(currencyPair, size);
        }

        #region Methods

        /// <summary>
        /// After an Order is accepted in the LimitOrderBook, adds the new order's attributes to the corresponding depth level
        /// </summary>
        /// <returns></returns>
        public bool OrderAccepted(Order order)
        {
            if (order.OrderType == OrderType.Limit)
            {
                _depth.AddOrder(order.Price, order.Volume, order.OrderSide);
                return true;
            }
            return false;
        }

        /// <summary>
        /// After an Order is cancelled in the LimitOrderBook, adds the new order's attributes to the corresponding depth level
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public bool OrderCancel(Order order)
        {
            return false;
        }

        /// <summary>
        /// After an Order is replaced in the LimitOrderBook, adds the new order's attributes to the corresponding depth level
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public bool OrderReplace(Order order)
        {
            return false;
        }

        /// <summary>
        /// After the OrderBook was updated, see if the depth we track was effected
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public bool OrderBookUpdated(Order order)
        {
            return false;
        }

        #endregion Methods

        #region Properties

        /// <summary>
        /// Currency Pair of this book
        /// </summary>
        public string CurrencyPair
        {
            get { return _currencyPair; }
        }

        #endregion Properties

        #region Implementation of Listeners

        /// <summary>
        /// When Order gets accepted
        /// </summary>
        /// <param name="order"></param>
        /// <param name="matchedPrice"></param>
        /// <param name="matchedVolume"></param>
        public void OnOrderAccepted(Order order, Price matchedPrice, Volume matchedVolume)
        {
            if (order.OrderType == OrderType.Limit)
            {
                if (matchedVolume != null && matchedVolume.Equals(order.Volume))
                {
                    _depth.IgnoreFillQuantity(matchedVolume, order.OrderSide);
                }
                else
                {
                    _depth.AddOrder(order.Price, order.Volume, order.OrderSide);
                }
            }
        }

        /// <summary>
        /// When order gets filled
        /// </summary>
        /// <param name="inboundOrder"></param>
        /// <param name="matchedOrder"> </param>
        /// <param name="fillFlags"> </param>
        /// <param name="filledPrice"></param>
        /// <param name="filledVolume"></param>
        public void OnOrderFilled(Order inboundOrder, Order matchedOrder, FillFlags fillFlags, Price filledPrice, Volume filledVolume)
        {
            bool isFilled = false;
            if (inboundOrder.OrderType == OrderType.Limit)
            {
                isFilled = fillFlags == FillFlags.InboundFilled || fillFlags == FillFlags.BothFilled;
                _depth.FillOrder(inboundOrder.Price, filledPrice, filledVolume, isFilled, inboundOrder.OrderSide);
            }
            if (matchedOrder.OrderType == OrderType.Limit)
            {
                isFilled = fillFlags == FillFlags.MatchedFilled || fillFlags == FillFlags.BothFilled;
                _depth.FillOrder(matchedOrder.Price, filledPrice, filledVolume, isFilled, matchedOrder.OrderSide);
            }
        }

        /// <summary>
        /// When Order gets cancelled
        /// </summary>
        /// <param name="order"> </param>
        public void OnOrderCancelled(Order order)
        {
            if (order.OrderType == OrderType.Limit)
            {
                _depth.CloseOrder(order.Price, order.OpenQuantity, order.OrderSide);
            }
        }

        /// <summary>
        /// Handlesthe event in case an order changes
        /// </summary>
        /// <param name="order"></param>
        public void OnOrderChanged(Order order)
        {
            
        }

        /// <summary>
        /// OnOrderBookChanged
        /// </summary>
        /// <param name="orderBook"></param>
        public void OnOrderBookChanged(LimitOrderBook orderBook)
        {
            if (_depth.Changed())
            {
                if (DepthChanged != null)
                {
                    DepthChanged(_depth);
                }
                if (BboChanged != null)
                {
                    int lastChange = _depth.LastPublishedChangeId;

                    if (_depth.BidLevels.First().ChangedSince(lastChange) || _depth.AskLevels.First().ChangedSince(lastChange))
                    {
                        BBO bbo = new BBO(_currencyPair, _depth.BidLevels.First(), _depth.AskLevels.First());
                        BboChanged(bbo);
                    }
                }
                _depth.Published();
            }
        }

        /// <summary>
        /// The Depth contained by this DepthOrderBook
        /// </summary>
        public Depth Depth { get { return _depth; } }

        #endregion
    }
}

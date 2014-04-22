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
    /// Handles the depths for the price levels in the Order Book
    /// </summary>
    public class DepthOrderBook : IOrderListener, IOrderBookListener, ITradeListener
    {
        private string _currencyPair = string.Empty;
        private int _size = 0;
        private Depth _depth = null;

        public event Action<DepthLevel> BboChanged;
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
        /// After an Order is filled in the LimitOrderBook, adds the new order's attributes to the corresponding depth level
        /// </summary>
        /// <param name="order"></param>
        /// <param name="filledVolume"></param>
        /// <param name="filledPrice"></param>
        /// <returns></returns>
        // ToDo: Hook this event from the exchange
        public void OrderFilled(Order order, Volume filledVolume, Price filledPrice)
        {
            if (order.OrderType == OrderType.Limit)
            {
                _depth.FillOrder(order.Price, filledPrice, filledVolume, order.OrderState == OrderState.Complete, order.OrderSide);
            }
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
        /// Handlesthe event in case an order changes
        /// </summary>
        /// <param name="order"></param>
        public void OnOrderChanged(Order order)
        {
            switch (order.OrderState)
            {
                case OrderState.Accepted:
                    OrderAccepted(order);
                    break;

                case OrderState.Cancelled:
                    OrderCancel(order);
                    break;
            }
        }

        /// <summary>
        /// OnOrderBookChanged
        /// </summary>
        /// <param name="orderBook"></param>
        public void OnOrderBookChanged(LimitOrderBook orderBook)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// On Trade execution
        /// </summary>
        /// <param name="trade"></param>
        public void OnTrade(Trade trade)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}

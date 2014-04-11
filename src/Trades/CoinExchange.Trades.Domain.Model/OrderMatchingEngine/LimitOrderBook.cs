using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Trades.Domain.Model.Order;
using CoinExchange.Trades.Domain.Model.Trades;

namespace CoinExchange.Trades.Domain.Model.MatchingEngine
{
    /// <summary>
    /// Book containing the limit orders for a particular currency pair
    /// </summary>
    public class LimitOrderBook
    {
        private string _currencyPair = string.Empty;

        /// <summary>
        /// Bid list that contains bids at the requested prices for every Depth Level
        /// Key = Price
        /// Value = Order
        /// </summary>
        private SortedList<decimal, Order.Order> _bids = new SortedList<decimal, Order.Order>();

        /// <summary>
        /// Ask list that contains asks at the requested prices for every Depth Level
        /// Key = Price
        /// Value = Order
        /// </summary>
        private SortedList<decimal, Order.Order> _asks = new SortedList<decimal, Order.Order>();

        private List<Trade> _trades = new List<Trade>(); 

        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="currencyPair"></param>
        public LimitOrderBook(string currencyPair)
        {
            _currencyPair = currencyPair;
        }

        #region Methods

        /// <summary>
        /// Place a new order on the order book
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public bool PlaceOrder(CoinExchange.Trades.Domain.Model.Order.Order order)
        {
            switch (order.IsSell)
            {
                case true:
                    return MatchSellOrder(order);

                case false:
                    return MatchBuyOrder(order);
            }
            return false;
        }

        /// <summary>
        /// Matches an offer to the avaiable bids. Produces trade and/or adds the new/partially left order to the list of bids/asks
        /// </summary>
        /// <returns></returns>
        private bool MatchSellOrder(Order.Order sellOrder)
        {
            IEnumerable<KeyValuePair<decimal, Order.Order>> matchingBids = from bid in _bids
                                                                           where bid.Value.LimitPrice >= sellOrder.LimitPrice
                                                                           select bid;

            List<KeyValuePair<decimal, Order.Order>> matchingBidsList = matchingBids as List<KeyValuePair<decimal, Order.Order>> ?? matchingBids.ToList();
            if (matchingBidsList.Any())
            {
                foreach (var matchingBid in matchingBidsList)
                {
                    if (sellOrder.Volume > 0)
                    {
                        sellOrder.Volume = sellOrder.Volume - matchingBid.Value.Volume;

                        // If the Sell Order's volume exceeds the Buy order's volume
                        if (sellOrder.Volume > 0)
                        {
                            // Send the Buy Order's volume as the quantity of the trade executed
                            Trade trade = GenerateTrade(matchingBid.Value.LimitPrice, matchingBid.Value.Volume,
                                                matchingBid.Value, sellOrder);

                            // ToDo: Create method that will generate a new TradeExecutedEvent and the Trade will raise it

                            sellOrder.Status = OrderStatus.PartiallyFilled;
                            matchingBid.Value.Status = OrderStatus.FullyFilled;
                            _bids.Remove(matchingBid.Key);
                        }
                        // If the Sell Order's volume is less than the Buy Order's volume
                        else if (sellOrder.Volume < 0)
                        {
                            // Send the Buy Order's volume as the quantity of the trade executed
                            Trade trade = GenerateTrade(matchingBid.Value.LimitPrice, -sellOrder.Volume,
                                                matchingBid.Value, sellOrder);

                            // ToDo: Create method that will generate a new TradeExecutedEvent and the Trade will raise it

                            matchingBid.Value.Volume = -sellOrder.Volume;

                            // ToDo: Raise an OrderUpdatedEvent over here

                            sellOrder.Status = OrderStatus.FullyFilled;
                            sellOrder.Volume = 0;
                        }
                        else if (sellOrder.Volume == 0)
                        {
                            // Send the Buy Order's volume as the quantity of the trade executed
                            Trade trade = GenerateTrade(matchingBid.Value.LimitPrice, matchingBid.Value.Volume,
                                                matchingBid.Value, sellOrder);

                            // ToDo: Create method that will generate a new TradeExecutedEvent and the Trade will raise it
                            return true;
                        }
                    }
                }
                if (sellOrder.Volume > 0)
                {
                    _asks.Add(sellOrder.LimitPrice, sellOrder);
                }
            }
            else
            {
                _asks.Add(sellOrder.LimitPrice, sellOrder);
            }
            return false;
        }

        /// <summary>
        /// Matches a bid to the available offers. Produces trade and/or adds the new/partially left order to the list of bids/asks
        /// </summary>
        /// <returns></returns>
        private bool MatchBuyOrder(Order.Order buyOrder)
        {
            // ToDo: Algorithm to match an incoming bids to the available offers
            // Need to divide the order between the opposite side orders available in the same price level
            // Add the offer to the offer list. Or bid in the bid list, if the bid quantity is left over
            return false;
        }

        /// <summary>
        /// Generates the Trade after a cross between two orders
        /// </summary>
        /// <returns></returns>
        private Trade GenerateTrade(decimal executionPrice, 
            decimal executedQuantity, Order.Order buyOrder, Order.Order sellOrder)
        {
            Trade trade = new Trade(buyOrder.Pair, executionPrice, executedQuantity, DateTime.Now, buyOrder,
                sellOrder);
            _trades.Add(trade);

            return trade;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Currency Pair of this book
        /// </summary>
        public string CurrencyPair
        {
            get { return _currencyPair; }
        }

        /// <summary>
        /// Bid list that contains bids at the requested prices for every Depth Level
        /// Key = Price
        /// Value = Order
        /// </summary>
        public SortedList<decimal, Order.Order> Bids
        {
            get { return _bids; }
        }

        /// <summary>
        /// Ask list that contains asks at the requested prices for every Depth Level
        /// Key = Price
        /// Value = Order
        /// </summary>
        public SortedList<decimal, Order.Order> Asks
        {
            get { return _asks; }
        }

        /// <summary>
        /// Contains the list of all trades
        /// </summary>
        public List<Trade> Trades
        {
            get { return _trades; }
        }

        #endregion Properties
    }
}

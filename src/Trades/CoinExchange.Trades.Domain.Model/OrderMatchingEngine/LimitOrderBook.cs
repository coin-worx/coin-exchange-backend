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
        /// Bid list
        /// </summary>
        private List<Order.Order> _bids = new List<Order.Order>();

        /// <summary>
        /// Ask list
        /// </summary>
        private List<Order.Order> _asks = new List<Order.Order>();

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
            switch (order.OrderSide)
            {
                case OrderSide.Sell:
                    return MatchSellOrder(order);

                case OrderSide.Buy:
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
            if (_bids.Any())
            {
                IEnumerable<Order.Order> matchingBids = from bid in _bids
                                                        where bid.Price >= sellOrder.Price
                                                        select bid;

                List<Order.Order> matchingBidsList = matchingBids as List<Order.Order> ?? matchingBids.ToList();
                if (matchingBidsList.Any())
                {
                    foreach (var matchingBid in matchingBidsList)
                    {
                        if (sellOrder.Volume > 0)
                        {
                            sellOrder.Volume = sellOrder.Volume - matchingBid.Volume;

                            // If the Sell Order's volume exceeds the Buy order's volume
                            if (sellOrder.Volume > 0)
                            {
                                // Send the Buy Order's volume as the quantity of the trade executed
                                Trade trade = GenerateTrade(matchingBid.Price, matchingBid.Volume,
                                                            matchingBid, sellOrder);

                                // ToDo: Need to figure out how to raise this event in the following method 
                                // and publish on the output disruptor
                                trade.RaiseEvent();
                                sellOrder.Status = OrderStatus.PartiallyFilled;
                                matchingBid.Status = OrderStatus.FullyFilled;
                                _bids.Remove(matchingBid);
                            }
                                // If the Sell Order's volume is less than the Buy Order's volume
                            else if (sellOrder.Volume < 0)
                            {
                                // Send the Buy Order's volume as the quantity of the trade executed
                                Trade trade = GenerateTrade(matchingBid.Price, -sellOrder.Volume,
                                                            matchingBid, sellOrder);

                                // ToDo: Need to figure out how to raise this event in the following method 
                                // and publish on the output disruptor
                                trade.RaiseEvent();
                                matchingBid.Volume = -sellOrder.Volume;

                                // ToDo: Raise an OrderUpdatedEvent over here

                                sellOrder.Status = OrderStatus.FullyFilled;
                                sellOrder.Volume = 0;
                            }
                            else if (sellOrder.Volume == 0)
                            {
                                // Send the Buy Order's volume as the quantity of the trade executed
                                Trade trade = GenerateTrade(matchingBid.Price, matchingBid.Volume,
                                                            matchingBid, sellOrder);

                                // ToDo: Need to figure out how to raise this event in the following method 
                                // and publish on the output disruptor
                                trade.RaiseEvent();
                                _bids.Remove(matchingBid);
                                return true;
                            }
                        }
                    }
                    if (sellOrder.Volume > 0)
                    {
                        _asks.Add(sellOrder);
                        _asks = _asks.OrderBy(x => x.Price).ToList();
                        return true;
                    }
                }
            }
            
            _asks.Add(sellOrder);
            _asks = _asks.OrderBy(x => x.Price).ToList();
            return true;
        }

        /// <summary>
        /// Matches a bid to the available offers. Produces trade and/or adds the new/partially left order to the list of bids/asks
        /// </summary>
        /// <returns></returns>
        private bool MatchBuyOrder(Order.Order buyOrder)
        {
            if (_asks.Any())
            {
                IEnumerable<Order.Order> matchingAsks = from ask in _asks
                                                        where ask.Price <= buyOrder.Price
                                                        select ask;

                List<Order.Order> matchingAskList = matchingAsks as List<Order.Order> ?? matchingAsks.ToList();
                if (matchingAskList.Any())
                {
                    foreach (var matchingAsk in matchingAskList)
                    {
                        if (buyOrder.Volume > 0)
                        {
                            buyOrder.Volume = buyOrder.Volume - matchingAsk.Volume;

                            // If the Sell Order's volume exceeds the Buy order's volume
                            if (buyOrder.Volume > 0)
                            {
                                // Send the Buy Order's volume as the quantity of the trade executed
                                Trade trade = GenerateTrade(matchingAsk.Price, matchingAsk.Volume,
                                                            matchingAsk, buyOrder);

                                // ToDo: Need to figure out how to raise this event in the following method 
                                // and publish on the output disruptor
                                trade.RaiseEvent();
                                buyOrder.Status = OrderStatus.PartiallyFilled;
                                matchingAsk.Status = OrderStatus.FullyFilled;
                                _asks.Remove(matchingAsk);
                            }
                            // If the Sell Order's volume is less than the Buy Order's volume
                            else if (buyOrder.Volume < 0)
                            {
                                // Send the Buy Order's volume as the quantity of the trade executed
                                Trade trade = GenerateTrade(matchingAsk.Price, -buyOrder.Volume,
                                                            matchingAsk, buyOrder);

                                // ToDo: Need to figure out how to raise this event in the following method 
                                // and publish on the output disruptor
                                trade.RaiseEvent();
                                matchingAsk.Volume = -buyOrder.Volume;

                                // ToDo: Raise an OrderUpdatedEvent over here

                                buyOrder.Status = OrderStatus.FullyFilled;
                                buyOrder.Volume = 0;
                            }
                            else if (buyOrder.Volume == 0)
                            {
                                // Send the Buy Order's volume as the quantity of the trade executed
                                Trade trade = GenerateTrade(matchingAsk.Price, matchingAsk.Volume,
                                                            matchingAsk, buyOrder);

                                // ToDo: Need to figure out how to raise this event in the following method 
                                // and publish on the output disruptor
                                trade.RaiseEvent();
                                _asks.Remove(matchingAsk);
                                return true;
                            }
                        }
                    }
                    if (buyOrder.Volume > 0)
                    {
                        _bids.Add(buyOrder);
                        _bids = _bids.OrderByDescending(x => x.Price).ToList();
                        return true;
                    }
                }
            }

            _bids.Add(buyOrder);
            _bids = _bids.OrderByDescending(x => x.Price).ToList();
            return true;
        }

        /// <summary>
        /// Generates the Trade after a cross between two orders
        /// </summary>
        /// <returns></returns>
        private Trade GenerateTrade(decimal executionPrice, 
            decimal executedQuantity, Order.Order buyOrder, Order.Order sellOrder)
        {
            Trade trade = new Trade(buyOrder.CurrencyPair, executionPrice, executedQuantity, DateTime.Now, buyOrder,
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
        /// Bid list
        /// </summary>
        public List<Order.Order> Bids
        {
            get { return _bids; }
        }

        /// <summary>
        /// Ask list
        /// </summary>
        public List<Order.Order> Asks
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

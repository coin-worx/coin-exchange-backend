using System;
using System.Collections.Generic;
using System.Linq;
using CoinExchange.Common.Domain.Model;
using CoinExchange.Trades.Domain.Model.Order;
using CoinExchange.Trades.Domain.Model.Trades;

namespace CoinExchange.Trades.Domain.Model.OrderMatchingEngine
{
    /// <summary>
    /// Book containing the limit orders for a particular currency pair
    /// </summary>
    public class LimitOrderBook
    {
        // Get the Current Logger
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger
        (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private string _currencyPair = string.Empty;
        private int _trasactionId = 0;

        /// <summary>
        /// Bid list
        /// </summary>
        private OrderList _bids = null;

        /// <summary>
        /// Ask list
        /// </summary>
        private OrderList _asks = null;

        /// <summary>
        /// List  Of Trades
        /// </summary>
        private List<Trade> _trades = new List<Trade>();

        // Listeners
        private TradeListener _tradeListener = null;
        private OrderListener _orderListener = null;
        private OrderBookListener _orderBookListener = null;
        private OrderAccepted _orderAccepted = null;
        private OrderRejected _orderRejected = null;

        // Events
        public event Action<Trade> TradeExecuted;
        public event Action<Order.Order> OrderChanged;
        public event Action<LimitOrderBook> OrderBookChanged;

        /// <summary>
        /// A delegate that sends notification to the client that the Order has been accepted
        /// </summary>
        public OrderAccepted OrderAccepted
        {
            get { return _orderAccepted; }
            set { _orderAccepted = value; }
        }

        /// <summary>
        /// A delegate that sends notification to the client that the Order has been rejected
        /// </summary>
        public OrderRejected OrderRejected
        {
            get { return _orderRejected; }
            set { _orderRejected = value; }
        }

        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="currencyPair"></param>
        public LimitOrderBook(string currencyPair)
        {
            _currencyPair = currencyPair;

            _bids = new OrderList(currencyPair, OrderSide.Buy);
            _asks = new OrderList(currencyPair, OrderSide.Sell);
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
                _trasactionId++;
                IEnumerable<Order.Order> matchingBids = from bid in _bids
                                                        where bid.Price.Value >= sellOrder.Price.Value
                                                        select bid;

                List<Order.Order> matchingBidsList = matchingBids as List<Order.Order> ?? matchingBids.ToList();
                if (matchingBidsList.Any())
                {
                    foreach (var matchingBid in matchingBidsList)
                    {
                        if (sellOrder.Volume.Value > 0)
                        {
                            sellOrder.Volume = new Volume(sellOrder.Volume.Value - matchingBid.Volume.Value);

                            // If the Sell Order's volume exceeds the Buy order's volume
                            if (sellOrder.Volume.Value > 0)
                            {
                                // Send the Buy Order's volume as the quantity of the trade executed
                                Trade trade = GenerateTrade(matchingBid.Price.Value, matchingBid.Volume.Value,
                                                            matchingBid, sellOrder);

                                // ToDo: Need to figure out how to raise this event in the following method 
                                // and publish on the output disruptor
                                trade.RaiseEvent();
                                sellOrder.Status = OrderStatus.PartiallyFilled;
                                matchingBid.Status = OrderStatus.FullyFilled;
                                _bids.Remove(matchingBid);
                            }
                                // If the Sell Order's volume is less than the Buy Order's volume
                            else if (sellOrder.Volume.Value < 0)
                            {
                                // Send the Buy Order's volume as the quantity of the trade executed
                                Trade trade = GenerateTrade(matchingBid.Price.Value, -sellOrder.Volume.Value,
                                                            matchingBid, sellOrder);

                                // ToDo: Need to figure out how to raise this event in the following method 
                                // and publish on the output disruptor
                                trade.RaiseEvent();
                                matchingBid.Volume = new Volume(-sellOrder.Volume.Value);

                                // ToDo: Raise an OrderUpdatedEvent over here

                                sellOrder.Status = OrderStatus.FullyFilled;
                                sellOrder.Volume = new Volume(0);
                            }
                            else if (sellOrder.Volume.Value == 0)
                            {
                                // Send the Buy Order's volume as the quantity of the trade executed
                                Trade trade = GenerateTrade(matchingBid.Price.Value, matchingBid.Volume.Value,
                                                            matchingBid, sellOrder);

                                // ToDo: Need to figure out how to raise this event in the following method 
                                // and publish on the output disruptor
                                trade.RaiseEvent();
                                _bids.Remove(matchingBid);
                                return true;
                            }
                        }
                    }
                    if (sellOrder.Volume.Value > 0)
                    {
                        _asks.Add(sellOrder);
                        return true;
                    }
                }
            }
            
            _asks.Add(sellOrder);
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
                _trasactionId++;
                IEnumerable<Order.Order> matchingAsks = from ask in _asks
                                                        where ask.Price.Value <= buyOrder.Price.Value
                                                        select ask;

                List<Order.Order> matchingAskList = matchingAsks as List<Order.Order> ?? matchingAsks.ToList();
                if (matchingAskList.Any())
                {
                    foreach (var matchingAsk in matchingAskList)
                    {
                        if (buyOrder.Volume.Value > 0)
                        {
                            buyOrder.Volume = new Volume(buyOrder.Volume.Value - matchingAsk.Volume.Value);

                            // If the Sell Order's volume exceeds the Buy order's volume
                            if (buyOrder.Volume.Value > 0)
                            {
                                // Send the Buy Order's volume as the quantity of the trade executed
                                Trade trade = GenerateTrade(matchingAsk.Price.Value, matchingAsk.Volume.Value,
                                                            matchingAsk, buyOrder);

                                // ToDo: Need to figure out how to raise this event in the following method 
                                // and publish on the output disruptor
                                trade.RaiseEvent();
                                buyOrder.Status = OrderStatus.PartiallyFilled;
                                matchingAsk.Status = OrderStatus.FullyFilled;
                                _asks.Remove(matchingAsk);
                            }
                            // If the Sell Order's volume is less than the Buy Order's volume
                            else if (buyOrder.Volume.Value < 0)
                            {
                                // Send the Buy Order's volume as the quantity of the trade executed
                                Trade trade = GenerateTrade(matchingAsk.Price.Value, -buyOrder.Volume.Value,
                                                            matchingAsk, buyOrder);

                                // ToDo: Need to figure out how to raise this event in the following method 
                                // and publish on the output disruptor
                                trade.RaiseEvent();
                                matchingAsk.Volume = new Volume(-buyOrder.Volume.Value);

                                // ToDo: Raise an OrderUpdatedEvent over here

                                buyOrder.Status = OrderStatus.FullyFilled;
                                buyOrder.Volume = new Volume(0);
                            }
                            else if (buyOrder.Volume.Value == 0)
                            {
                                // Send the Buy Order's volume as the quantity of the trade executed
                                Trade trade = GenerateTrade(matchingAsk.Price.Value, matchingAsk.Volume.Value,
                                                            matchingAsk, buyOrder);

                                // ToDo: Need to figure out how to raise this event in the following method 
                                // and publish on the output disruptor
                                trade.RaiseEvent();
                                _asks.Remove(matchingAsk);
                                return true;
                            }
                        }
                    }
                    if (buyOrder.Volume.Value > 0)
                    {
                        _bids.Add(buyOrder);
                        return true;
                    }
                }
            }

            _bids.Add(buyOrder);
            return true;
        }

        /// <summary>
        /// Cancel the order
        /// </summary>
        public bool CancelOrder(OrderId orderId)
        {
            Order.Order foundAsk = _asks.FindOrder(orderId);
            if (foundAsk != null)
            {
                _asks.Remove(foundAsk);
                return true;
            }
            Order.Order foundBid = _bids.FindOrder(orderId);
            if (foundBid != null)
            {
                _bids.Remove(foundBid);
                return true;
            }
            return false;
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
        /// Contains the list of all trades
        /// </summary>
        public List<Trade> Trades
        {
            get { return _trades; }
        }

        /// <summary>
        /// Listens to the Trades that are executed by the Order Book
        /// </summary>
        public TradeListener TradeListener
        {
            get
            {
                return _tradeListener;
            }
            set
            {
                AssertionConcern.AssertArgumentNotNull(value, "TradeListener provided is equal to null.");
                _tradeListener = value;
                if (TradeExecuted == null)
                {
                    TradeExecuted += _tradeListener.OnTradeExecuted;
                }
            }
        }

        /// <summary>
        /// Listens to the changes in the states of Orders
        /// </summary>
        public OrderListener OrderListener
        {
            get
            {
                return _orderListener;
            } 
            set
            {
                AssertionConcern.AssertArgumentNotNull(value, "OrderListener provided is equal to null.");
                _orderListener = value;
                if (OrderChanged == null)
                {
                    OrderChanged += _orderListener.OnOrderChanged;
                }
            }
        }

        /// <summary>
        /// Listens to the changes in the states of this OrderBook
        /// </summary>
        public OrderBookListener OrderBookListener
        {
            get
            {
                return _orderBookListener;
            }
            set
            {
                AssertionConcern.AssertArgumentNotNull(value, "OrderBookListener provided is equal to null.");
                _orderBookListener = value;
                if (OrderBookChanged == null)
                {
                    OrderBookChanged += _orderBookListener.OnOrderBookChanged;
                }
            }
        }

        /// <summary>
        /// The Bids list. Methods to add, modify, remove are internal so cannot be called outside the assembly
        /// </summary>
        public OrderList Bids
        {
            get { return _bids; }
        }

        /// <summary>
        /// The Asks list. Methods to add, modify, remove are internal so cannot be called outside the assembly
        /// </summary>
        public OrderList Asks
        {
            get { return _asks; }
        }

        /// <summary>
        /// The number of Bids present in the Bids list
        /// </summary>
        public int BidCount
        {
            get { return _bids.Count(); }
        }

        /// <summary>
        /// The number of Asks present in the Bids list
        /// </summary>
        public int AskCount
        {
            get { return _asks.Count(); }
        }

        #endregion Properties
    }
}

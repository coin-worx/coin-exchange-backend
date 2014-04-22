using System;
using System.Collections.Generic;
using System.Linq;
using CoinExchange.Common.Domain.Model;
using CoinExchange.Trades.Domain.Model.OrderAggregate;
using CoinExchange.Trades.Domain.Model.TradeAggregate;

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
        private DepthOrderBook _depthOrderBook = null;
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
        public event Action<Order> OrderChanged;
        public event Action<LimitOrderBook> OrderBookChanged;
        public event Action<Order> OrderAccepted;
        public event Action<Order, Volume, Price> OrderFilled;

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
        /// Matches the incoming order to the list of available orders
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public bool AddOrder(Order order)
        {
            // If order is valid, otherwise, IsValid method updates order state as rejected and sends back notification to the client
            if (IsValid(order))
            {
                PlaceOrder(order);
            }
            
            return false;
        }

        /// <summary>
        /// Place a new order on the order book
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public bool PlaceOrder(Order order)
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
        /// Validates if the prices match for the criteria specified for each level
        /// </summary>
        /// <param name="inboundPrice"></param>
        /// <param name="currentPrice"></param>
        /// <param name="orderSide"></param>
        /// <returns></returns>
        public bool Matched(Price inboundPrice, Price currentPrice, OrderSide orderSide)
        {
            switch (orderSide)
            {
                case OrderSide.Buy:
                    if (inboundPrice >= currentPrice)
                    {
                        return true;
                    }
                    return false;
                case OrderSide.Sell:
                    if (inboundPrice <= currentPrice)
                    {
                        return true;
                    }
                    return false;
            }
            return false;
        }

        //------------------------------------------------------------------------------------------------
        /// <summary>
        /// Matches an offer to the avaiable bids. Produces trade and/or adds the new/partially left order to the list of bids/asks
        /// </summary>
        /// <returns></returns>
        private bool MatchSellOrder(Order sellOrder)
        {
            if (_bids.Any())
            {
                _trasactionId++;
                IEnumerable<Order> matchingBids = from bid in _bids
                                                        where bid.Price.Value >= sellOrder.Price.Value
                                                        select bid;

                List<Order> matchingBidsList = matchingBids as List<Order> ?? matchingBids.ToList();
                if (matchingBidsList.Any())
                {
                    return MatchOrder(sellOrder, _asks, matchingBidsList, _bids);
                }
            }
            
            _asks.Add(sellOrder);
            return true;
        }

        /// <summary>
        /// Matches a bid to the available offers. Produces trade and/or adds the new/partially left order to the list of bids/asks
        /// </summary>
        /// <returns></returns>
        private bool MatchBuyOrder(Order buyOrder)
        {
            if (_asks.Any())
            {
                _trasactionId++;
                IEnumerable<Order> matchingAsks = from ask in _asks
                                                        where ask.Price.Value <= buyOrder.Price.Value
                                                        select ask;

                List<Order> matchingAskList = matchingAsks as List<Order> ?? matchingAsks.ToList();
                if (matchingAskList.Any())
                {
                    return MatchOrder(buyOrder, _bids, matchingAskList, _asks);
                }
            }

            _bids.Add(buyOrder);
            return true;
        }

        /// <summary>
        /// Matches the inbound order to the order in the opposite side's list. E.g., in case of inbound ask order, each 
        /// bid in the buy order book will be matched to it
        /// </summary>
        /// <param name="order"></param>
        /// <param name="inboundOrderSideList"></param>
        /// <param name="oppositeMatchingOrdersList"></param>
        /// <param name="oppositeOrderSideList"></param>
        /// <returns></returns>
        public bool MatchOrder(Order order, OrderList inboundOrderSideList, 
            List<Order> oppositeMatchingOrdersList, OrderList oppositeOrderSideList)
        {
            decimal currentVolume = order.Volume.Value;
            foreach (var matchingOrder in oppositeMatchingOrdersList)
            {
                if (currentVolume > 0)
                {
                    currentVolume = currentVolume - matchingOrder.Volume.Value;

                    // If the inbound Order's volume exceeds the opposite side order's volume present in the list
                    if (currentVolume > 0)
                    {
                        // Send the opposite side Order's volume as the quantity of the trade executed
                        Trade trade = GenerateTrade(matchingOrder.Price.Value, matchingOrder.Volume.Value,
                                                    matchingOrder, order);

                        // ToDo: Need to figure out how to raise this event in the following method 
                        // and publish on the output disruptor
                        trade.RaiseEvent();
                        // Fill price is price the matching order in the list
                        order.UpdateVolume(new Volume(currentVolume));
                        Fill(order, matchingOrder);
                        oppositeOrderSideList.Remove(matchingOrder);
                    }
                    // If the inbound Order's volume is less than the opposite side Order's volume in the list
                    else if (currentVolume < 0)
                    {
                        // Send the inbound order's quantity as the quantity executed
                        Trade trade = GenerateTrade(matchingOrder.Price.Value, order.Volume.Value,
                                                    matchingOrder, order);

                        // ToDo: Need to figure out how to raise this event in the following method
                        // and publish on the output disruptor
                        trade.RaiseEvent();
                        UpdateOrderInDepth(matchingOrder, new Volume(-currentVolume));

                        // ToDo: Raise an OrderUpdatedEvent over here

                        // Fill price is price the matching order in the list
                        Fill(order, matchingOrder);
                    }
                    else if (currentVolume == 0)
                    {
                        // Send the opposite side Order's volume as the quantity of the trade executed
                        Trade trade = GenerateTrade(matchingOrder.Price.Value, matchingOrder.Volume.Value,
                                                    matchingOrder, order);

                        // ToDo: Need to figure out how to raise this event in the following method 
                        // and publish on the output disruptor
                        trade.RaiseEvent();
                        // Fill price is price the matching order in the list
                        Fill(order, matchingOrder);
                        oppositeOrderSideList.Remove(matchingOrder);
                        return true;
                    }
                }
            }
            if (order.Volume.Value > 0)
            {
                inboundOrderSideList.Add(order);
                // ToDo: Add to Depth
                return true;
            }
            return false;
        }

        /// <summary>
        /// Cancel the order
        /// </summary>
        public bool CancelOrder(OrderId orderId)
        {
            Order foundAsk = _asks.FindOrder(orderId);
            if (foundAsk != null)
            {
                _asks.Remove(foundAsk);
                return true;
            }
            Order foundBid = _bids.FindOrder(orderId);
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
            decimal executedQuantity, Order buyOrder, Order sellOrder)
        {
            Trade trade = new Trade(buyOrder.CurrencyPair, new Price(executionPrice), new Volume(executedQuantity),
                DateTime.Now, buyOrder, sellOrder);
            _trades.Add(trade);

            return trade;
        }

        /// <summary>
        /// Validates wheterh the order is valid or not
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public bool IsValid(Order order)
        {
            if (order.Volume.Value == 0)
            {
                order.Rejected();
                if (OrderChanged != null)
                {
                    OrderChanged(order);
                }
                return false;
            }
            else
            {
                order.Accepted();
                if (OrderAccepted != null)
                {
                    OrderAccepted(order);
                }
                return true;
            }
        }

        /// <summary>
        /// Gets the price for the inbound order, can depend on various circumstances that can be added later
        /// </summary>
        /// <returns></returns>
        public Price SortPrice(Order order)
        {
            return order.Price;
        }

        #region Order Updating methods

        /// <summary>
        /// Change quantity for an order
        /// </summary>
        public void ChangeQuantity(Order order, Volume newVolume)
        {
            AssertionConcern.AssertGreaterThanZero(newVolume.Value, "New volume for the order is less than zero");
            order.UpdateVolume(newVolume);
        }

        /// <summary>
        /// Add a new order to its corresponding depth level
        /// </summary>
        /// <param name="order"></param>
        public void AddOrderToDepth(Order order)
        {
            if (OrderChanged != null)
            {
                OrderChanged(order);
            }
        }

        public void UpdateOrderInDepth(Order order, Volume newVolume)
        {
            ChangeQuantity(order, newVolume);
            if (OrderChanged != null)
            {
                // The depthOrderBook does not need to get this new volume, the order already contains the new volume
                OrderChanged(order);
            }
        }

        /// <summary>
        /// Update the order according to the fill
        /// </summary>
        /// <param name="matchedOrder"></param>
        /// <param name="inboundOrder"> </param>
        public void Fill(Order inboundOrder, Order matchedOrder)
        {
            decimal filledQuantity = Math.Min(inboundOrder.OpenQuantity.Value, matchedOrder.OpenQuantity.Value);
            decimal filledPrice = matchedOrder.Price.Value;
            Price inboundFilledCost = new Price(filledQuantity * filledPrice);
            inboundOrder.Fill(new Volume(filledQuantity), inboundFilledCost);
            if (OrderChanged != null)
            {
                OrderChanged(inboundOrder);
            }

            Price matchedFilledCost = new Price(filledQuantity * filledPrice);
            matchedOrder.Fill(new Volume(filledQuantity), matchedFilledCost);
            if (OrderFilled != null)
            {
                OrderFilled(matchedOrder, new Volume(filledQuantity), new Price(filledPrice));
            }

            // Create trade. The least amount of the two orders will be the trade's executed volume
            Trade trade = GenerateTrade(filledPrice, filledQuantity, matchedOrder, inboundOrder);

            // ToDo: Need to figure out how to raise this event in the following method 
            // and publish on the output disruptor
            trade.RaiseEvent();
        }

        #endregion Order Updating methods

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
                    TradeExecuted += _tradeListener.OnTrade;
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

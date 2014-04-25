using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Trades.Domain.Model.DomainEvents;
using CoinExchange.Trades.Domain.Model.OrderAggregate;
using CoinExchange.Trades.Domain.Model.OrderMatchingEngine;

namespace CoinExchange.Trades.ReadModel.MemoryImages
{
    /// <summary>
    /// The memory image containing the OrderBook in memory
    /// </summary>
    public class OrderBookMemoryImage
    {
        // Get the Current Logger
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger
        (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private List<string> _currencyPairs = new List<string>(); 
        private LimitOrderBook _limitOrderBook = null;
        private OrderRepresentationBookList _bidBookRepresentations = null;
        private OrderRepresentationBookList _askBookRepresentations = null;

        /// <summary>
        /// Default Constructor
        /// </summary>
        public OrderBookMemoryImage()
        {
            _bidBookRepresentations = new OrderRepresentationBookList();
            _askBookRepresentations = new OrderRepresentationBookList();
            InitializeCurrencyPairs();
            InitializeOrderBooksRepresentations();
            LimitOrderBookEvent.LimitOrderBookChanged += OnOrderBookChanged;
        }

        /// <summary>
        /// Initialize the set of currency pairs that the application is to support.
        /// </summary>
        private void InitializeCurrencyPairs()
        {
            _currencyPairs.Add(CurrencyConstants.BitCoinUsd);
        }

        /// <summary>
        /// Initialize lists that will contain the OrderBook representations for all currency pairs
        /// </summary>
        private void InitializeOrderBooksRepresentations()
        {
            foreach (var currencyPair in _currencyPairs)
            {
                OrderRepresentationList bidRepresentationList = new OrderRepresentationList(currencyPair, OrderSide.Buy);
                OrderRepresentationList askRepresentationList = new OrderRepresentationList(currencyPair, OrderSide.Sell);

                _bidBookRepresentations.AddRecord(bidRepresentationList);
                _askBookRepresentations.AddRecord(askRepresentationList);
            }
        }

        /// <summary>
        /// Receives the Order Book from the output Disruptor
        /// </summary>
        /// <param name="orderBook"></param>
        public void OnOrderBookChanged(LimitOrderBook orderBook)
        {
            UpdateBids(orderBook.Bids);
            UpdateAsks(orderBook.Asks);
        }

        private void UpdateBids(OrderList orderList)
        {
            var orderRepresentationList = GetOrderRepresentationList(_bidBookRepresentations, orderList);
            UpdateOrderRepresentationList(_bidBookRepresentations, orderRepresentationList, orderList);
        }

        private void UpdateAsks(OrderList orderList)
        {
            var orderRepresentationList = GetOrderRepresentationList(_askBookRepresentations, orderList);
            UpdateOrderRepresentationList(_askBookRepresentations, orderRepresentationList, orderList);
        }

        /// <summary>
        /// Updates the OrderBookRepresentation. Used by both bid and aask representations
        /// </summary>
        private OrderRepresentationList GetOrderRepresentationList(OrderRepresentationBookList orderBookRepresentations, OrderList orderList)
        {
            foreach (OrderRepresentationList orderRepresentationList in orderBookRepresentations)
            {
                if (orderRepresentationList.CurrencyPair == orderList.CurrencyPair)
                {
                    return orderRepresentationList;
                }
            }
            return null;
        }

        /// <summary>
        /// Updates the OrderRepresentationlist with new values
        /// </summary>
        /// <returns></returns>
        private void UpdateOrderRepresentationList(OrderRepresentationBookList orderRepresentationBook, OrderRepresentationList orderRepresentationList, OrderList orderList)
        {
            orderRepresentationBook.Remove(orderRepresentationList.CurrencyPair);
            orderRepresentationList = new OrderRepresentationList(orderList.CurrencyPair, orderList.OrderSide);

            foreach (Order order in orderList)
            {
                orderRepresentationList.AddRecord(order.Volume.Value, order.Price.Value);
                Log.Debug("Order Representation added to Currency Pair : " + order.CurrencyPair + 
                          ". Volume: " + order.Volume.Value + " | Price: " + order.Price.Value);   
            }
            orderRepresentationBook.AddRecord(orderRepresentationList);
        }

        /// <summary>
        /// Represents the Bid Books
        /// </summary>
        public OrderRepresentationBookList BidBooks
        {
            get { return _bidBookRepresentations; }
        }

        public OrderRepresentationBookList AskBooks
        {
            get { return _askBookRepresentations; }
        }
    }
}
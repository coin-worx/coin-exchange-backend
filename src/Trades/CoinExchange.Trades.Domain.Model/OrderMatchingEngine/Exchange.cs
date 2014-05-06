using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Trades.Domain.Model.OrderAggregate;
using CoinExchange.Trades.Domain.Model.Services;
using Disruptor;

namespace CoinExchange.Trades.Domain.Model.OrderMatchingEngine
{
    /// <summary>
    /// Initializes andcontains all Order Books and forwards requests for submitting and cancelling orders
    /// </summary>
    [Serializable]
    public class Exchange:IEventHandler<InputPayload>
    {
        // Get the Current Logger
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger
        (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private string BitCoinUsd = "BTCUSD";
        List<string> _currencyPairs = new List<string>();
        private ExchangeEssentialsList _exchangeEssentialsList = new ExchangeEssentialsList();

        /// <summary>
        /// Default Constructor
        /// </summary>
        public Exchange()
        {
            _currencyPairs.Add(BitCoinUsd);
            foreach (var currencyPair in _currencyPairs)
            {
                LimitOrderBook orderBook = new LimitOrderBook(currencyPair);
                DepthOrderBook depthOrderBook = new DepthOrderBook(currencyPair, 5);

                TradeListener tradeListener = new TradeListener();
                IOrderListener orderListener = new OrderListener();
                IOrderBookListener orderBookListener = new OrderBookListener();
                IBBOListener bboListener = new BBOListener();
                IDepthListener depthListener = new DepthListener();

                orderBook.OrderAccepted -= OnAccept;
                orderBook.OrderAccepted -= depthOrderBook.OnOrderAccepted;

                orderBook.OrderAccepted += OnAccept;
                orderBook.OrderAccepted += depthOrderBook.OnOrderAccepted;

                orderBook.OrderCancelled -= depthOrderBook.OnOrderCancelled;
                orderBook.OrderCancelled += depthOrderBook.OnOrderCancelled;

                orderBook.OrderBookChanged -= depthOrderBook.OnOrderBookChanged;
                orderBook.OrderBookChanged -= orderBookListener.OnOrderBookChanged;

                orderBook.OrderBookChanged += depthOrderBook.OnOrderBookChanged;
                orderBook.OrderBookChanged += orderBookListener.OnOrderBookChanged;

                orderBook.OrderChanged -= depthOrderBook.OnOrderChanged;
                orderBook.OrderChanged -= orderListener.OnOrderChanged;

                orderBook.OrderChanged += depthOrderBook.OnOrderChanged;
                orderBook.OrderChanged += orderListener.OnOrderChanged;

                orderBook.OrderFilled -= depthOrderBook.OnOrderFilled;
                orderBook.OrderFilled += depthOrderBook.OnOrderFilled;

                orderBook.TradeExecuted -= tradeListener.OnTrade;
                orderBook.TradeExecuted += tradeListener.OnTrade;

                depthOrderBook.BboChanged -= bboListener.OnBBOChange;
                depthOrderBook.DepthChanged -= depthListener.OnDepthChanged;

                depthOrderBook.BboChanged += bboListener.OnBBOChange;
                depthOrderBook.DepthChanged += depthListener.OnDepthChanged;

                _exchangeEssentialsList.AddEssentials(new ExchangeEssentials(orderBook, depthOrderBook, tradeListener,
                    orderListener, depthListener, bboListener));
            }
        }

        #region Methods

        /// <summary>
        /// Places new order on the limit order book
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public bool PlaceNewOrder(Order order)
        {
            switch (order.CurrencyPair)
            {
                case "BTCUSD":
                    return _exchangeEssentialsList.First().LimitOrderBook.PlaceOrder(order);
            }
            return false;
        }

        /// <summary>
        /// Cancel the order with the given orderId
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public bool CancelOrder(OrderId orderId)
        {
            // ToDo: Ask Bilal to provide the currency pair as well here in order for the exchange to figure out which order
            // book to call
            return _exchangeEssentialsList.First().LimitOrderBook.CancelOrder(orderId);
        }

        /// <summary>
        /// Signifies that the Order has been accepted successfully
        /// </summary>
        public void OnAccept(Order order, Price matchedPrice, Volume matchedVolume)
        {
            Log.Debug("Order Accepted by Exchange. " + order.ToString());
            // Note: the notification to the client can be sent back from here
        }

        /// <summary>
        /// Start the replay of events for each LimitOrderBook to rebuild them from scratch
        /// </summary>
        /// <param name="journaler"></param>
        public void StartReplay(Journaler journaler)
        {
            TurnReplayModeOn();
            foreach (var exchangeEssential in _exchangeEssentialsList)
            {
                var ordersForReplay = journaler.GetOrdersForReplay(exchangeEssential.LimitOrderBook);
                foreach (var order in ordersForReplay)
                {
                    if (order.OrderState == OrderState.Accepted)
                    {
                        this.PlaceNewOrder(order);
                    }
                    else if (order.OrderState == OrderState.Cancelled)
                    {
                        this.CancelOrder(order.OrderId);
                    }
                }
            }

            TurnReplayModeOff();
        }

        /// <summary>
        /// Turns on the replay mode and unsubscribes the OrderListener from listeneing to event while the replay mode is running
        /// </summary>
        public void TurnReplayModeOn()
        {
            // Unsubscribe each order listener from each LimitOrderBook while replaying is in order
            foreach (ExchangeEssentials exchangeEssentials in ExchangeEssentials)
            {
                exchangeEssentials.LimitOrderBook.OrderChanged -= exchangeEssentials.DepthOrderBook.OnOrderChanged;
                exchangeEssentials.LimitOrderBook.OrderChanged -= exchangeEssentials.OrderListener.OnOrderChanged;
            }
        }

        /// <summary>
        /// Turns replay mode off 
        /// </summary>
        public void TurnReplayModeOff()
        {
            foreach (ExchangeEssentials exchangeEssentials in ExchangeEssentials)
            {
                exchangeEssentials.LimitOrderBook.OrderChanged += exchangeEssentials.DepthOrderBook.OnOrderChanged;
                exchangeEssentials.LimitOrderBook.OrderChanged += exchangeEssentials.OrderListener.OnOrderChanged;
            }
        }

        #endregion Methods

        /// <summary>
        /// ExchangeEssentials
        /// </summary>
        public ExchangeEssentialsList ExchangeEssentials
        {
            get { return _exchangeEssentialsList; }
            private set { _exchangeEssentialsList = value; }
        }

        /// <summary>
        /// Receives new order and cancel order requests from disruptor
        /// </summary>
        /// <param name="data"></param>
        /// <param name="sequence"></param>
        /// <param name="endOfBatch"></param>
        public void OnNext(InputPayload data, long sequence, bool endOfBatch)
        {
            if (data.IsOrder)
            {
                Order order=new Order();
                data.Order.MemberWiseClone(order);
                PlaceNewOrder(order);
            }
            else
            {
                OrderCancellation cancellation=new OrderCancellation();
                data.OrderCancellation.MemberWiseClone(cancellation);
                //TODO: Modify cancel order function to order cancellation type
                CancelOrder(cancellation.OrderId);
            }
        }
    }
}

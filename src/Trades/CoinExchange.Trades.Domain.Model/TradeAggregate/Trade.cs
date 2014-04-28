using System;
using CoinExchange.Common.Domain.Model;
using CoinExchange.Trades.Domain.Model.OrderAggregate;

namespace CoinExchange.Trades.Domain.Model.TradeAggregate
{
    /// <summary>
    /// Result of a bid and ask crossing
    /// </summary>
    [Serializable]
    public class Trade
    {
        private readonly string _aggregateId;
        private string _currencyPair = string.Empty;
        private Price _executionPrice = null;
        private Volume _executedQuantity = null;
        private DateTime _executionTime = DateTime.MinValue;
        private Order _buyOrder = null;
        private Order _sellOrder = null;
        private TradeId _tradeId;

        /// <summary>
        /// Default Constructor
        /// </summary>
        public Trade(string currencyPair, Price executionPrice, Volume executedQuantity, DateTime executionTime,
            Order matchedOrder, Order inboundOrder)
        {
            _currencyPair = currencyPair;
            _executionPrice = executionPrice;
            _executedQuantity = executedQuantity;
            _executionTime = executionTime;
            if (matchedOrder.OrderSide == OrderSide.Buy)
            {
                _buyOrder = matchedOrder;
                _sellOrder = inboundOrder;
            }
            else
            {
                _buyOrder = inboundOrder;
                _sellOrder = matchedOrder;
            }

            // ToDo: Need to implement auto incremental aggregate Id generator 
        }

        /// <summary>
        /// Factory Constructor
        /// </summary>
        public Trade(TradeId tradeId,string currencyPair, Price executionPrice, Volume executedQuantity, DateTime executionTime,
            Order matchedOrder, Order inboundOrder)
        {
            TradeId = tradeId;
            _currencyPair = currencyPair;
            _executionPrice = executionPrice;
            _executedQuantity = executedQuantity;
            _executionTime = executionTime;
            if (matchedOrder.OrderSide == OrderSide.Buy)
            {
                _buyOrder = matchedOrder;
                _sellOrder = inboundOrder;
            }
            else
            {
                _buyOrder = inboundOrder;
                _sellOrder = matchedOrder;
            }
        }

        /// <summary>
        /// Raise the TradeExecutedEvent
        /// </summary>
        public TradeExecutedEvent RaiseEvent()
        {
            TradeExecutedEvent tradeExecutedEvent = new TradeExecutedEvent(_aggregateId, this);
            return tradeExecutedEvent;
        }

        /// <summary>
        /// TradeId
        /// </summary>
        public TradeId TradeId
        {
            get { return _tradeId; }
            private set
            {
                AssertionConcern.AssertArgumentNotNull(value,"TradeId cannot be null");
                _tradeId = value;
            }
        }

        /// <summary>
        /// Currency Pair
        /// </summary>
        public string CurrencyPair
        {
            get { return _currencyPair; }
        }

        /// <summary>
        /// Execution Price
        /// </summary>
        public Price ExecutionPrice
        {
            get { return _executionPrice; }
        }

        /// <summary>
        /// Executed Quantity
        /// </summary>
        public Volume ExecutedVolume
        {
            get { return _executedQuantity; }
        }

        /// <summary>
        /// ExecutionTime
        /// </summary>
        public DateTime ExecutionTime
        {
            get { return _executionTime; }
        }

        /// <summary>
        /// Buy Order Reference
        /// </summary>
        public Order BuyOrder
        {
            get { return _buyOrder; }
        }

        /// <summary>
        /// Sell Order Reference
        /// </summary>
        public Order SellOrder
        {
            get { return _sellOrder; }
        }
    }
}

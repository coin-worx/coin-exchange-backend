using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CoinExchange.Common.Domain.Model;
using CoinExchange.Trades.Domain.Model.Trades;

namespace CoinExchange.Trades.Domain.Model.Order
{
    /// <summary>
    /// VO that will represent cancelation request
    /// </summary>
    public class CancelOrder
    {
        private TraderId _traderId;
        private OrderId _orderId;

        public OrderId OrderId
        {
            get { return _orderId; }
            private set
            {
                AssertionConcern.AssertArgumentNotNull(value, "Order id cannot be null");
                _orderId = value;
            }
        }

        public TraderId TraderId
        {
            get { return _traderId; }
            private set
            {
                AssertionConcern.AssertArgumentNotNull(value,"Trader id cannot be null");
                _traderId = value;
            }
        }

        public CancelOrder()
        {
            
        }

        public CancelOrder(OrderId orderId, TraderId traderId)
        {
            OrderId = orderId;
            TraderId = traderId;
        }

       public override bool Equals(object obj)
        {
            CancelOrder cancelOrder = obj as CancelOrder;
            if (cancelOrder == null)
            {
                return false;
            }
            return (cancelOrder.OrderId.Id == this.OrderId.Id && cancelOrder.TraderId.Id == this.TraderId.Id);
        }

        /// <summary>
        /// Perform deep copy
        /// </summary>
        /// <param name="cancelOrder"></param>
        /// <returns></returns>
        public CancelOrder MemberWiseClone(CancelOrder cancelOrder)
        {
            cancelOrder.OrderId = new OrderId(OrderId.Id);
            cancelOrder.TraderId=new TraderId(TraderId.Id);
            return cancelOrder;
        }
    }
}

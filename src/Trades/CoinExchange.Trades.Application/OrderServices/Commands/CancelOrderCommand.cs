using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Common.Domain.Model;
using CoinExchange.Trades.Domain.Model.OrderAggregate;
using CoinExchange.Trades.Domain.Model.TradeAggregate;

namespace CoinExchange.Trades.Application.OrderServices.Commands
{
    public class CancelOrderCommand
    {
        public OrderId OrderId { get; private set; }
        public TraderId TraderId { get; private set; }

        public CancelOrderCommand(OrderId orderId, TraderId traderId)
        {
            AssertionConcern.AssertArgumentNotNull(orderId,"OrderId not provided or it is invalid");
            AssertionConcern.AssertArgumentNotNull(traderId,"TraderId not provided or it is invalid");
            OrderId = orderId;
            TraderId = traderId;
        }
    }
}

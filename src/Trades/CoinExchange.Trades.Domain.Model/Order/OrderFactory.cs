using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Common.Specifications;
using CoinExchange.Trades.Domain.Model.Services;
using CoinExchange.Trades.Domain.Model.Trades;
using Spring.Context;

namespace CoinExchange.Trades.Domain.Model.Order
{
    /// <summary>
    /// serves the purpose for creating a valid order
    /// </summary>
    public static class OrderFactory
    {
        public static Order CreateOrder(string traderId,string currencyPair,string type, string side,decimal volume,decimal limitPrice, IOrderIdGenerator orderIdGenerator)
        {
            Order order=null;
            TraderId id=new TraderId(int.Parse(traderId));
            OrderId orderId = orderIdGenerator.GenerateOrderId();
            if (side.Equals(Constants.OrderSide.Buy, StringComparison.CurrentCultureIgnoreCase) &&
                type.Equals(Constants.OrderType.MarketOrder, StringComparison.CurrentCultureIgnoreCase))
            {
                order=BuyOrder(orderId,currencyPair,limitPrice,OrderType.Market, volume,id);
                
            }
            else if (side.Equals(Constants.OrderSide.Sell, StringComparison.CurrentCultureIgnoreCase) &&
                     type.Equals(Constants.OrderType.MarketOrder, StringComparison.CurrentCultureIgnoreCase))
            {
                order = SellOrder(orderId, currencyPair, limitPrice, OrderType.Market, volume, id);
                
            }
            else if (side.Equals(Constants.OrderSide.Buy, StringComparison.CurrentCultureIgnoreCase) &&
                     type.Equals(Constants.OrderType.LimitOrder, StringComparison.CurrentCultureIgnoreCase))
            {
                order = BuyOrder(orderId, currencyPair, limitPrice, OrderType.Limit, volume, id);
                
            }
            else if (side.Equals(Constants.OrderSide.Sell, StringComparison.CurrentCultureIgnoreCase) &&
                     type.Equals(Constants.OrderType.LimitOrder, StringComparison.CurrentCultureIgnoreCase))
            {
                order = SellOrder(orderId, currencyPair, limitPrice, OrderType.Limit, volume, id);
                
            }

            //TODO:Validation of funds and other things
            
            return order;
        }

        private static Order BuyOrder(OrderId orderId, string pair, decimal limitPrice, OrderType orderType, decimal volume, TraderId traderId)
        {
            return new Order(orderId,pair,limitPrice,OrderSide.Buy,orderType,volume,traderId);
        }

        private static Order SellOrder(OrderId orderId, string pair, decimal limitPrice,OrderType orderType, decimal volume, TraderId traderId)
        {
            return new Order(orderId, pair, limitPrice, OrderSide.Sell, orderType, volume, traderId);
        }
        
    }
}

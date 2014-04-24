using System;
using CoinExchange.Common.Domain.Model;
using CoinExchange.Trades.Domain.Model.Services;
using CoinExchange.Trades.Domain.Model.TradeAggregate;

namespace CoinExchange.Trades.Domain.Model.OrderAggregate
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
            Volume orderVolume=new Volume(volume);
            if (side.Equals(Constants.ORDER_SIDE_BUY, StringComparison.CurrentCultureIgnoreCase) &&
                type.Equals(Constants.ORDER_TYPE_MARKET, StringComparison.CurrentCultureIgnoreCase))
            {
                order = BuyMarketOrder(orderId, currencyPair, OrderType.Market, orderVolume, id);                
            }
            else if (side.Equals(Constants.ORDER_SIDE_SELL, StringComparison.CurrentCultureIgnoreCase) &&
                     type.Equals(Constants.ORDER_TYPE_MARKET, StringComparison.CurrentCultureIgnoreCase))
            {
                order = SellMarketOrder(orderId, currencyPair, OrderType.Market, orderVolume, id);               
            }
            else if (side.Equals(Constants.ORDER_SIDE_BUY, StringComparison.CurrentCultureIgnoreCase) &&
                     type.Equals(Constants.ORDER_TYPE_LIMIT, StringComparison.CurrentCultureIgnoreCase))
            {
                Price price = new Price(limitPrice);
                order = BuyLimitOrder(orderId, currencyPair, price, OrderType.Limit, orderVolume, id);                
            }
            else if (side.Equals(Constants.ORDER_SIDE_SELL, StringComparison.CurrentCultureIgnoreCase) &&
                     type.Equals(Constants.ORDER_TYPE_LIMIT, StringComparison.CurrentCultureIgnoreCase))
            {
                Price price = new Price(limitPrice);
                order = SellLimitOrder(orderId, currencyPair, price, OrderType.Limit, orderVolume, id);             
            }

            //TODO:Validation of funds and other things
            
            return order;
        }

        private static Order BuyMarketOrder(OrderId orderId, string pair, OrderType orderType, Volume volume, TraderId traderId)
        {
            return new Order(orderId, pair, OrderSide.Buy, orderType, volume, traderId);
        }

        private static Order SellMarketOrder(OrderId orderId, string pair, OrderType orderType, Volume volume, TraderId traderId)
        {
            return new Order(orderId, pair, OrderSide.Sell, orderType, volume, traderId);
        }
        private static Order BuyLimitOrder(OrderId orderId, string pair, Price limitPrice, OrderType orderType, Volume volume, TraderId traderId)
        {
            return new Order(orderId, pair, limitPrice, OrderSide.Buy, orderType, volume, traderId);
        }

        private static Order SellLimitOrder(OrderId orderId, string pair, Price limitPrice, OrderType orderType, Volume volume, TraderId traderId)
        {
            return new Order(orderId, pair, limitPrice, OrderSide.Sell, orderType, volume, traderId);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Common.Specifications;

namespace CoinExchange.Trades.Domain.Model.Order
{
    /// <summary>
    /// serves the purpose for creating a valid order
    /// </summary>
    public static class OrderFactory
    {
        public static Order CreateOrder(string traderId,string currencyPair,string type, string side,decimal volume,decimal price, ISpecification<Order> specification)
        {
            Order order=null;
            if(side.Equals("buy",StringComparison.CurrentCultureIgnoreCase)&&type.Equals("market",StringComparison.CurrentCultureIgnoreCase))
                order = new Order(currencyPair, price, OrderSide.Buy, OrderType.Market, volume, traderId);
            else if (side.Equals("sell", StringComparison.CurrentCultureIgnoreCase) && type.Equals("market", StringComparison.CurrentCultureIgnoreCase))
                order = new Order(currencyPair, price, OrderSide.Buy, OrderType.Market, volume, traderId);
            else if (side.Equals("buy", StringComparison.CurrentCultureIgnoreCase) && type.Equals("limit", StringComparison.CurrentCultureIgnoreCase))
                order = new Order(currencyPair, price, OrderSide.Buy, OrderType.Market, volume, traderId);
            else if (side.Equals("sell", StringComparison.CurrentCultureIgnoreCase) && type.Equals("limit", StringComparison.CurrentCultureIgnoreCase))
                order = new Order(currencyPair, price, OrderSide.Buy, OrderType.Market, volume, traderId);
            //validate order
            specification.IsSatisfiedBy(order);
            return order;
        }
        
    }
}

/***************************************************************************** 
* Copyright 2016 Aurora Solutions 
* 
*    http://www.aurorasolutions.io 
* 
* Aurora Solutions is an innovative services and product company at 
* the forefront of the software industry, with processes and practices 
* involving Domain Driven Design(DDD), Agile methodologies to build 
* scalable, secure, reliable and high performance products.
* 
* Coin Exchange is a high performance exchange system specialized for
* Crypto currency trading. It has different general purpose uses such as
* independent deposit and withdrawal channels for Bitcoin and Litecoin,
* but can also act as a standalone exchange that can be used with
* different asset classes.
* Coin Exchange uses state of the art technologies such as ASP.NET REST API,
* AngularJS and NUnit. It also uses design patterns for complex event
* processing and handling of thousands of transactions per second, such as
* Domain Driven Designing, Disruptor Pattern and CQRS With Event Sourcing.
* 
* Licensed under the Apache License, Version 2.0 (the "License"); 
* you may not use this file except in compliance with the License. 
* You may obtain a copy of the License at 
* 
*    http://www.apache.org/licenses/LICENSE-2.0 
* 
* Unless required by applicable law or agreed to in writing, software 
* distributed under the License is distributed on an "AS IS" BASIS, 
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. 
* See the License for the specific language governing permissions and 
* limitations under the License. 
*****************************************************************************/


﻿using System;
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
            TraderId id=new TraderId(traderId);
            OrderId orderId = orderIdGenerator.GenerateOrderId();
            Volume orderVolume=new Volume(volume);
            if (side.Equals(Constants.ORDER_SIDE_BUY, StringComparison.CurrentCultureIgnoreCase) &&
                type.Equals(Constants.ORDER_TYPE_MARKET, StringComparison.CurrentCultureIgnoreCase))
            {
                Price price = new Price(0);
                order = BuyMarketOrder(orderId, currencyPair, OrderType.Market, orderVolume, price, id);                
            }
            else if (side.Equals(Constants.ORDER_SIDE_SELL, StringComparison.CurrentCultureIgnoreCase) &&
                     type.Equals(Constants.ORDER_TYPE_MARKET, StringComparison.CurrentCultureIgnoreCase))
            {
                Price price = new Price(0);
                order = SellMarketOrder(orderId, currencyPair, OrderType.Market, orderVolume, price, id);               
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

        private static Order BuyMarketOrder(OrderId orderId, string pair, OrderType orderType, Volume volume, Price price, TraderId traderId)
        {
            return new Order(orderId, pair, price, OrderSide.Buy, orderType, volume, traderId);
        }

        private static Order SellMarketOrder(OrderId orderId, string pair, OrderType orderType, Volume volume, Price price, TraderId traderId)
        {
            return new Order(orderId, pair, price,  OrderSide.Sell, orderType, volume, traderId);
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

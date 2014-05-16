using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using CoinExchange.Trades.Domain.Model.OrderAggregate;
using CoinExchange.Trades.Domain.Model.Services;

namespace CoinExchange.Trades.Domain.Model.OrderMatchingEngine
{
    /// <summary>
    /// Fetches the orders to replay from the Journaler and then forwards them to the Exchange
    /// </summary>
    public class LimitOrderBookReplayService
    {
        /// <summary>
        /// Replays events for the LimitOrderBooks to rebuild them to their prior state
        /// </summary>
        public void ReplayOrderBooks(Exchange exchange, Journaler journaler)
        {
            exchange.TurnReplayModeOn();
            foreach (var exchangeEssential in exchange.ExchangeEssentials)
            {
                List<Order> ordersForReplay = journaler.GetOrdersForReplay(exchangeEssential.LimitOrderBook);

                if (ordersForReplay != null)
                {
                    Order[] deepCopy = (Order[]) GetCopy(ordersForReplay.ToArray());
                    foreach (var order in deepCopy)
                    {
                        if (order.OrderState == OrderState.Accepted)
                        {
                            exchange.PlaceNewOrder(order);
                        }
                        else if (order.OrderState == OrderState.Cancelled)
                        {
                            exchange.CancelOrder(new OrderCancellation(order.OrderId, order.TraderId, order.CurrencyPair));
                        }
                    }
                }
            }
            exchange.TurnReplayModeOff();
        }

        /// <summary>
        /// Create a copy of the incoming object
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private static object GetCopy(object input)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, input);
                stream.Position = 0;
                return formatter.Deserialize(stream);
            }
        }
    }
}

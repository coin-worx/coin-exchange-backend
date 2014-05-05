using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Trades.Domain.Model.OrderAggregate;
using CoinExchange.Trades.Domain.Model.OrderMatchingEngine;
using CoinExchange.Trades.Domain.Model.Services;

namespace CoinExchange.Trades.Application.MatchingEngineServices
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
                var ordersForReplay = journaler.GetOrdersForReplay(exchangeEssential.LimitOrderBook);

                foreach (var order in ordersForReplay)
                {
                    if (order.OrderState == OrderState.Accepted)
                    {
                        exchange.PlaceNewOrder(order);
                    }
                    else if (order.OrderState == OrderState.Cancelled)
                    {
                        exchange.CancelOrder(order.OrderId);
                    }
                }
            }
            exchange.TurnReplayModeOff();
        }
    }
}

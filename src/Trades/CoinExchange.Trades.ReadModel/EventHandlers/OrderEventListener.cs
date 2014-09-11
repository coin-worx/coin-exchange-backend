using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Common.Utility;
using CoinExchange.Trades.Domain.Model.OrderAggregate;
using CoinExchange.Trades.Domain.Model.Services;
using CoinExchange.Trades.ReadModel.DTO;
using CoinExchange.Trades.ReadModel.Repositories;

namespace CoinExchange.Trades.ReadModel.EventHandlers
{
    /// <summary>
    /// Listens order change events
    /// </summary>
    public class OrderEventListener
    {
        private IPersistanceRepository _persistanceRepository;
        private IBalanceValidationService _balanceValidationService;

        /// <summary>
        /// Parametrized Constructor
        /// </summary>
        /// <param name="persistanceRepository"></param>
        /// <param name="balanceValidationService"></param>
        public OrderEventListener(IPersistanceRepository persistanceRepository, IBalanceValidationService balanceValidationService)
        {
            _persistanceRepository = persistanceRepository;
            _balanceValidationService = balanceValidationService;
            OrderEvent.OrderChanged += OnOrderStatusChanged;
        }

        /// <summary>
        /// Handles the event of change in orders
        /// </summary>
        /// <param name="order"></param>
        void OnOrderStatusChanged(Order order)
        {
            _persistanceRepository.SaveOrUpdate(ReadModelAdapter.GetOrderReadModel(order));

            // If the order has been cancelled, send the info to Funds BC so that the funds can be updated
            if (order.OrderState == OrderState.Cancelled)
            {
                // First split the curreny pair into base and quote currency
                Tuple<string, string> splittedCurrencyPair =
                    CurrencySplitterService.SplitCurrencyPair(order.CurrencyPair);
                if (!string.IsNullOrEmpty(splittedCurrencyPair.Item1) &&
                    !string.IsNullOrEmpty(splittedCurrencyPair.Item2))
                {
                    // Send to the Infrastructure service which will communicate cross Bounded Context
                    _balanceValidationService.OrderCancelled(splittedCurrencyPair.Item1, splittedCurrencyPair.Item2,
                                                             order.TraderId.Id, order.OrderSide.ToString(),
                                                             order.OrderId.Id,
                                                             order.OpenQuantity.Value, order.Price.Value);
                }
            }
        }
    }
}

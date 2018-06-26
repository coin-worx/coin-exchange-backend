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

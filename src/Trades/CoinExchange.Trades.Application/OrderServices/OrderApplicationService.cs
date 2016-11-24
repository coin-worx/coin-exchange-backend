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
using System.IO;
using CoinExchange.Common.Domain.Model;
using CoinExchange.Common.Utility;
using CoinExchange.Trades.Application.OrderServices.Commands;
using CoinExchange.Trades.Application.OrderServices.Representation;
using CoinExchange.Trades.Domain.Model.OrderAggregate;
using CoinExchange.Trades.Domain.Model.Services;
using Spring.Context;
using Spring.Context.Support;

namespace CoinExchange.Trades.Application.OrderServices
{
    /// <summary>
    /// Real implementation of order application service
    /// </summary>
    public class OrderApplicationService : IOrderApplicationService
    {
        private ICancelOrderCommandValidation _commandValidationService;
        private IBalanceValidationService _balanceValidationService;

        public OrderApplicationService(ICancelOrderCommandValidation cancelOrderCommandValidation, 
            dynamic balanceValidationService)
        {
            _commandValidationService = cancelOrderCommandValidation;
            _balanceValidationService = balanceValidationService;
        }

        public CancelOrderResponse CancelOrder(CancelOrderCommand cancelOrderCommand)
        {
            try
            {
                // Verify cancel order command
                if (_commandValidationService.ValidateCancelOrderCommand(cancelOrderCommand))
                {
                    string currencyPair = _commandValidationService.GetCurrencyPair(cancelOrderCommand.OrderId);
                    OrderCancellation cancellation = new OrderCancellation(cancelOrderCommand.OrderId,
                                                                           cancelOrderCommand.TraderId,currencyPair);
                    InputDisruptorPublisher.Publish(InputPayload.CreatePayload(cancellation));
                    return new CancelOrderResponse(true, "Cancel Request Accepted");
                }
                return new CancelOrderResponse(false, new InvalidDataException("Invalid orderid").ToString());
            }
            catch (Exception exception)
            {
                return new CancelOrderResponse(false, exception.Message);
            }
        }

        public NewOrderRepresentation CreateOrder(CreateOrderCommand orderCommand)
        {
            IOrderIdGenerator orderIdGenerator = ContextRegistry.GetContext()["OrderIdGenerator"] as IOrderIdGenerator;

            Tuple<string, string> splittedCurrencyPairs = CurrencySplitterService.SplitCurrencyPair(orderCommand.Pair);
            
            Order order = OrderFactory.CreateOrder(orderCommand.TraderId, orderCommand.Pair,
                orderCommand.Type, orderCommand.Side, orderCommand.Volume, orderCommand.Price, orderIdGenerator);

            if (_balanceValidationService.FundsConfirmation(order.TraderId.Id, splittedCurrencyPairs.Item1,
                splittedCurrencyPairs.Item2, order.Volume.Value, order.Price.Value, order.OrderSide.ToString(),
                order.OrderId.Id))
            {
                InputDisruptorPublisher.Publish(InputPayload.CreatePayload(order));
                return new NewOrderRepresentation(order);
            }
            throw new InvalidOperationException("Not Enough Balance for Trader with ID: " + orderCommand.TraderId);
        }
    }
}

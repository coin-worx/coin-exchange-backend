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
using CoinExchange.Funds.Domain.Model.DepositAggregate;
using CoinExchange.Funds.Domain.Model.Services;
using CoinExchange.Funds.Domain.Model.CurrencyAggregate;

namespace CoinExchange.Funds.Application.OrderValidationServices
{
    /// <summary>
    /// Service for Validating if the user has enough balance to send the current order
    /// </summary>
    public class OrderValidationApplicationService : IOrderValidationApplicationService
    {
        private IFundsValidationService _fundsValidationService = null;

        /// <summary>
        /// Default Constructor
        /// </summary>
        public OrderValidationApplicationService(IFundsValidationService fundsValidationService)
        {
            _fundsValidationService = fundsValidationService;
        }

        /// <summary>
        /// Validates the funds before sending an order
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="baseCurrency"></param>
        /// <param name="baseCurrencyIsCrypto"></param>
        /// <param name="quoteCurrency"></param>
        /// <param name="quoteCurrencyIsCrypto"></param>
        /// <param name="volume"></param>
        /// <param name="price"></param>
        /// <param name="orderSide"></param>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public bool ValidateFundsForOrder(int accountId, string baseCurrency, bool baseCurrencyIsCrypto, 
            string quoteCurrency, bool quoteCurrencyIsCrypto, decimal volume, decimal price, string orderSide, 
            string orderId)
        {
            return _fundsValidationService.ValidateFundsForOrder(new AccountId(accountId), 
                                                                 new Currency(baseCurrency, baseCurrencyIsCrypto),
                                                                 new Currency(quoteCurrency, quoteCurrencyIsCrypto),
                                                                 volume, price, orderSide, orderId);
        }

        /// <summary>
        /// Updates the balance after sending the order
        /// </summary>
        /// <param name="baseCurrency"></param>
        /// <param name="baseCurrencyIsCrypto"></param>
        /// <param name="quoteCurrency"></param>
        /// <param name="quoteCurrencyIsCrypto"></param>
        /// <param name="tradeVolume"></param>
        /// <param name="price"></param>
        /// <param name="executionDateTime"></param>
        /// <param name="tradeId"></param>
        /// <param name="buyAccountId"></param>
        /// <param name="sellAccountId"></param>
        /// <param name="buyOrderId"></param>
        /// <param name="sellOrderId"></param>
        public bool TradeExecuted(string baseCurrency, bool baseCurrencyIsCrypto, string quoteCurrency, 
            bool quoteCurrencyIsCrypto, decimal tradeVolume, decimal price, DateTime executionDateTime, string tradeId,
            int buyAccountId, int sellAccountId, string buyOrderId, string sellOrderId)
        {
            return _fundsValidationService.TradeExecuted(baseCurrency, quoteCurrency, tradeVolume, price, executionDateTime,
                                                  tradeId, buyAccountId, sellAccountId, buyOrderId, sellOrderId);
        }

        /// <summary>
        /// Restores the balance when an order is cancelled based on the order's open quantity
        /// </summary>
        /// <param name="baseCurrency"></param>
        /// <param name="quoteCurrency"></param>
        /// <param name="accountId"></param>
        /// <param name="orderSide"></param>
        /// <param name="orderId"></param>
        /// <param name="openQuantity"></param>
        /// <param name="price"></param>
        /// <returns></returns>
        public bool OrderCancelled(string baseCurrency, string quoteCurrency, int accountId, string orderSide, string orderId,
            decimal openQuantity, decimal price)
        {
            return _fundsValidationService.OrderCancelled(new Currency(baseCurrency), new Currency(quoteCurrency),
                                                          new AccountId(accountId), orderSide, orderId, openQuantity, price);
        }
    }
}

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

namespace CoinExchange.Funds.Application.OrderValidationServices
{
    /// <summary>
    /// Interface for the validation operations on the applciation layer level
    /// </summary>
    public interface IOrderValidationApplicationService
    {
        /// <summary>
        /// Confirm that the user has enough funds to send the current order to the exchange
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="baseCurrency"></param>
        /// <param name="baseCurrencyIsCrypto"> </param>
        /// <param name="quoteCurrency"></param>
        /// <param name="quoteCurrencyIsCrypto"> </param>
        /// <param name="volume"></param>
        /// <param name="price"></param>
        /// <param name="orderSide"></param>
        /// <param name="orderId"></param>
        /// <returns></returns>
        bool ValidateFundsForOrder(int accountId, string baseCurrency, bool baseCurrencyIsCrypto, string quoteCurrency,
                               bool quoteCurrencyIsCrypto, decimal volume, decimal price, string orderSide, string orderId);

        /// <summary>
        /// Informs that a trade has been executed and the Funds BC should update the corresponding balance
        /// </summary>
        /// <param name="baseCurrency"></param>
        /// <param name="baseCurrencyIsCrypto"> </param>
        /// <param name="quoteCurrency"></param>
        /// <param name="quoteCurrencyIsCrypto"> </param>
        /// <param name="tradeVolume"></param>
        /// <param name="price"></param>
        /// <param name="executionDateTime"></param>
        /// <param name="tradeId"></param>
        /// <param name="buyAccountId"></param>
        /// <param name="sellAccountId"></param>
        /// <param name="buyOrderId"></param>
        /// <param name="sellOrderId"></param>
        /// <returns></returns>
        bool TradeExecuted(string baseCurrency, bool baseCurrencyIsCrypto, string quoteCurrency, bool quoteCurrencyIsCrypto, 
                        decimal tradeVolume, decimal price, DateTime executionDateTime, string tradeId, int buyAccountId, 
                        int sellAccountId, string buyOrderId, string sellOrderId);

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
        bool OrderCancelled(string baseCurrency, string quoteCurrency, int accountId, string orderSide, string orderId,
                            decimal openQuantity, decimal price);
    }
}

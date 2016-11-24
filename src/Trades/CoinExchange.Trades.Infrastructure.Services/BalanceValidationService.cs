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
using CoinExchange.Trades.Domain.Model.Services;

namespace CoinExchange.Trades.Infrastructure.Services
{
    public class BalanceValidationService : IBalanceValidationService
    {
        private dynamic _orderValidationApplicationService = null;

        /// <summary>
        /// Parameterized Constructor
        /// </summary>
        public BalanceValidationService(dynamic orderValidationApplicationService)
        {
            _orderValidationApplicationService = orderValidationApplicationService;
        }

        /// <summary>
        /// Confirms if enough funds are present to send this order
        /// </summary>
        /// <returns></returns>
        public bool FundsConfirmation(string accountId, string baseCurrency, string quoteCurrency,
            decimal volume, decimal price, string orderSide, string orderId)
        {
            return _orderValidationApplicationService.ValidateFundsForOrder(Convert.ToInt32(accountId), baseCurrency, true/*Default: CryptoCurrency*/, 
            quoteCurrency, false/*Default: Fiat Currency*/, volume, price, orderSide, orderId);
        }

        /// <summary>
        /// Informs the Funds BC that a trade has executed which in turn updates the balancefor the involving currrencies
        /// </summary>
        /// <param name="baseCurrency"></param>
        /// <param name="quoteCurrency"></param>
        /// <param name="tradeVolume"></param>
        /// <param name="price"></param>
        /// <param name="executionDateTime"></param>
        /// <param name="tradeId"></param>
        /// <param name="buyAccountId"></param>
        /// <param name="sellAccountId"></param>
        /// <param name="buyOrderId"></param>
        /// <param name="sellOrderId"></param>
        public bool TradeExecuted(string baseCurrency, string quoteCurrency, decimal tradeVolume, decimal price, 
            DateTime executionDateTime, string tradeId, string buyAccountId, string sellAccountId, string buyOrderId, 
            string sellOrderId)
        {
            return _orderValidationApplicationService.TradeExecuted(baseCurrency, true/*Default Base Currency = Crypto*/,
                                                                    quoteCurrency, false/*Default Quote CUrrency = Fiat*/,
                                                                    tradeVolume, price, executionDateTime, tradeId, 
                                                                    int.Parse(buyAccountId), int.Parse(sellAccountId),
                                                                    buyOrderId, sellOrderId);
        }


        /// <summary>
        /// Calls Funds BC to restore balance after the order cancellation
        /// </summary>
        /// <param name="baseCurrency"></param>
        /// <param name="quoteCurrency"></param>
        /// <param name="traderId"></param>
        /// <param name="orderSide"></param>
        /// <param name="orderId"></param>
        /// <param name="openQuantity"></param>
        /// <param name="price"></param>
        /// <returns></returns>
        public bool OrderCancelled(string baseCurrency, string quoteCurrency, string traderId, string orderSide, string orderId, decimal openQuantity, decimal price)
        {
            return _orderValidationApplicationService.OrderCancelled(baseCurrency, quoteCurrency, int.Parse(traderId), orderSide, orderId,
                                                                openQuantity, price);
        }
    }
}

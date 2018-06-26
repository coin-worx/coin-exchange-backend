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
using CoinExchange.Funds.Domain.Model.DepositAggregate;
using CoinExchange.Funds.Domain.Model.LedgerAggregate;
using CoinExchange.Funds.Domain.Model.WithdrawAggregate;
using CoinExchange.Funds.Domain.Model.CurrencyAggregate;

namespace CoinExchange.Funds.Domain.Model.Services
{
    /// <summary>
    /// Interface for Funds validation
    /// </summary>
    public interface IFundsValidationService
    {
        /// <summary>
        /// Validates the Funds before sending an order
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="baseCurrency"></param>
        /// <param name="quoteCurrency"> </param>
        /// <param name="volume"></param>
        /// <param name="price"> </param>
        /// <param name="orderSide"></param>
        /// <param name="orderId"> </param>
        /// <returns></returns>
        bool ValidateFundsForOrder(AccountId accountId, CurrencyAggregate.Currency baseCurrency, 
            CurrencyAggregate.Currency quoteCurrency, decimal volume, decimal price, string orderSide, string orderId);

        /// <summary>
        /// Validates that enough funds exist for the requested withdrawal to be made
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="currency"></param>
        /// <param name="amount"></param>
        /// <param name="transactionId"> </param>
        /// <param name="bitcoinAddress"> </param>
        /// <returns></returns>
        Withdraw ValidateFundsForWithdrawal(AccountId accountId, CurrencyAggregate.Currency currency, decimal amount, 
            TransactionId transactionId, BitcoinAddress bitcoinAddress);

        /// <summary>
        /// Handles the event that withdraw has been confirmed and takes the necessary steps
        /// </summary>
        /// <param name="withdraw"> </param>
        /// <returns></returns>
        bool WithdrawalExecuted(Withdraw withdraw);

        /// <summary>
        /// Handles the event that a Deposit has been made and performes the necesssary options
        /// </summary>
        /// <param name="deposit"> </param>
        /// <returns></returns>
        bool DepositConfirmed(Deposit deposit);

        /// <summary>
        /// Is the Deposit within the limits and required tier level is verified for this user
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="currency"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        bool IsDepositLegit(AccountId accountId, Currency currency, decimal amount);

        /// <summary>
        /// Handles the event that a trade has been executed and performs the necessay steps to update the balance and 
        /// create a transaction record
        /// </summary>
        /// <param name="baseCurrency"></param>
        /// <param name="quoteCurrency"> </param>
        /// <param name="tradeVolume"></param>
        /// <param name="price"></param>
        /// <param name="executionDateTime"></param>
        /// <param name="tradeId"></param>
        /// <param name="buyAccountId"></param>
        /// <param name="sellAccountId"></param>
        /// <param name="buyOrderId"></param>
        /// <param name="sellOrderId"></param>
        /// <returns></returns>
        bool TradeExecuted(string baseCurrency, string quoteCurrency, decimal tradeVolume, decimal price,
            DateTime executionDateTime, string tradeId, int buyAccountId, int sellAccountId, string buyOrderId,
            string sellOrderId);

        /// <summary>
        /// Updates the balance when the order is cancelled
        /// </summary>
        /// <param name="quoteCurrency"> </param>
        /// <param name="accountId"></param>
        /// <param name="orderside"> </param>
        /// <param name="orderId"></param>
        /// <param name="openQuantity"> </param>
        /// <param name="baseCurrency"> </param>
        /// <param name="price"> </param>
        /// <returns></returns>
        bool OrderCancelled(Currency baseCurrency, Currency quoteCurrency, AccountId accountId, string orderside,
            string orderId, decimal openQuantity, decimal price);

        /// <summary>
        /// Evaluates and returns the original threshold limits and the threshold limits used
        /// </summary>
        /// <param name="accountId"> </param>
        /// <param name="currency"> </param>
        /// <returns></returns>
        AccountDepositLimits GetDepositLimitThresholds(AccountId accountId, Currency currency);

        /// <summary>
        /// Gets the original threashld for day and month, alon with the limits that have been used by the user
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="currency"></param>
        /// <returns></returns>
        AccountWithdrawLimits GetWithdrawThresholds(AccountId accountId, Currency currency);

        /// <summary>
        /// Confirms if the user has been verified for the transaction on the current tier level
        /// </summary>
        /// <returns></returns>
        Tuple<bool, string> IsTierVerified(int accountId, bool isCrypto);
    }
}

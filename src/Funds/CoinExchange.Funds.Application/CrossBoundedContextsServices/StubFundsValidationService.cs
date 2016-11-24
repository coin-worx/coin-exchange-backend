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
using CoinExchange.Funds.Domain.Model.CurrencyAggregate;
using CoinExchange.Funds.Domain.Model.DepositAggregate;
using CoinExchange.Funds.Domain.Model.Services;
using CoinExchange.Funds.Domain.Model.WithdrawAggregate;

namespace CoinExchange.Funds.Application.CrossBoundedContextsServices
{
    /// <summary>
    /// Stub Implementation for FundsVlaidationService
    /// </summary>
    public class StubFundsValidationService : IFundsValidationService
    {
        public bool ValidateFundsForOrder(AccountId accountId, Currency baseCurrency, Currency quoteCurrency, decimal volume, decimal price, string orderSide, string orderId)
        {
            return true;
        }

        public Withdraw ValidateFundsForWithdrawal(AccountId accountId, Currency currency, decimal amount, TransactionId transactionId, BitcoinAddress bitcoinAddress)
        {
            throw new NotImplementedException();
        }

        public bool WithdrawalExecuted(Withdraw withdraw)
        {
            return true;
        }

        public bool DepositConfirmed(Deposit deposit)
        {
            return true;
        }

        public bool IsDepositLegit(AccountId accountId, Currency currency, decimal amount)
        {
            return true;
        }

        public bool TradeExecuted(string baseCurrency, string quoteCurrency, decimal tradeVolume, decimal price, DateTime executionDateTime, string tradeId, int buyAccountId, int sellAccountId, string buyOrderId, string sellOrderId)
        {
            throw new NotImplementedException();
        }

        public bool OrderCancelled(Currency baseCurrency, Currency quoteCurrency, AccountId accountId, string orderside, string orderId, decimal openQuantity, decimal price)
        {
            throw new NotImplementedException();
        }

        public AccountDepositLimits GetDepositLimitThresholds(AccountId accountId, Currency currency)
        {
            throw new NotImplementedException();
        }

        public AccountWithdrawLimits GetWithdrawThresholds(AccountId accountId, Currency currency)
        {
            throw new NotImplementedException();
        }

        public Tuple<bool, string> IsTierVerified(int accountId, bool isCrypto)
        {
            return new Tuple<bool, string>(true, "Tier 1");
        }
    }
}

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
using CoinExchange.Funds.Application.DepositServices.Commands;
using CoinExchange.Funds.Application.DepositServices.Representations;

namespace CoinExchange.Funds.Application.DepositServices
{
    /// <summary>
    /// Interface for the Deposit Application service
    /// </summary>
    public interface IDepositApplicationService
    {
        /// <summary>
        /// Invoked when a deposit transaction is confirmed
        /// </summary>
        /// <param name="transactionId"></param>
        /// <param name="confirmations"></param>
        void OnDepositConfirmed(string transactionId, int confirmations);

        /// <summary>
        /// Invoked when a new Deposit Transaction arrives. Tuple in the list argument contains:
        /// Item1 = Address, Item2 = TransactionId, Item3 = Amount, Item4 = Category
        /// </summary>
        /// <param name="currency"></param>
        /// <param name="newTransactions"></param>
        void OnDepositArrival(string currency, List<Tuple<string, string, decimal, string>> newTransactions);

        /// <summary>
        /// Get recent deposits for a given currency and account ID
        /// </summary>
        /// <returns></returns>
        List<DepositRepresentation> GetRecentDeposits(string currency, int accountId);

        /// <summary>
        /// Gets new address from the BitcoinD Service for making a deposit
        /// </summary>
        /// <returns></returns>
        DepositAddressRepresentation GenarateNewAddress(GenerateNewAddressCommand generateNewAddressCommand);

        /// <summary>
        /// Get the threshold limits of deposit for this user
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="currency"></param>
        /// <returns></returns>
        DepositLimitThresholdsRepresentation GetThresholdLimits(int accountId, string currency);

        /// <summary>
        /// Get the list of addresses for the current user
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="currency"> </param>
        /// <returns></returns>
        IList<DepositAddressRepresentation> GetAddressesForAccount(int accountId, string currency);

        /// <summary>
        /// Make a deposit for a currency. This feature is not present to be called by front end
        /// </summary>
        /// <param name="makeDepositCommand"> </param>
        /// <returns></returns>
        bool MakeDeposit(MakeDepositCommand makeDepositCommand);

        /// <summary>
        /// Get the Monthly and daily Tier Limits for Deposit
        /// </summary>
        /// <returns></returns>
        DepositTierLimitRepresentation GetDepositTiersLimits();
    }
}

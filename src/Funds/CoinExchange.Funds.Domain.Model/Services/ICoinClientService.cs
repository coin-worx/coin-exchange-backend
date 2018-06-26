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


﻿
using System;
using System.Collections.Generic;
using System.Timers;

namespace CoinExchange.Funds.Domain.Model.Services
{
    /// <summary>
    /// Interface for interacting with the Bitcoin Client for deposits and withdrawals
    /// </summary>
    public interface ICoinClientService
    {
        /// <summary>
        /// Invoked when a new deposit is received, Param1 = Currency, 
        /// Param2 = List(Tuple): Item1 = BitcoinAddress, Item2 = TransacitonId, Item3 = Amount, Item4 = Category
        /// </summary>
        event Action<string, List<Tuple<string, string, decimal, string>>> DepositArrived;

        /// <summary>
        /// Invoked when a Deposit is confirmed. Param1 = TransactionId, Param2 = No. of Confirmations
        /// </summary>
        event Action<string, int> DepositConfirmed;

        /// <summary>
        /// Creates a new address using the Coin Client,for wither the Deposit or Withdraw
        /// </summary>
        /// <returns></returns>
        string CreateNewAddress();

        /// <summary>
        /// Receives Withdraw, forwards to Bitcoin Client to proceed. Returns Transaction ID if commit is made
        /// </summary>
        /// <returns></returns>
        string CommitWithdraw(string bitcoinAddress, decimal amount);

        /// <summary>
        /// Check the balance for the wallet held by the Exchange
        /// </summary>
        /// <param name="currency"></param>
        /// <returns></returns>
        decimal CheckBalance(string currency);

        /// <summary>
        /// Interval for polling
        /// </summary>
        double PollingInterval { get; }

        /// <summary>
        /// The currency that this CoinClientService implementation represents
        /// </summary>
        string Currency { get; }

        /// <summary>
        /// Interval for checking new transactions
        /// </summary>
        double NewTransactionsInterval { get; }
    }
}

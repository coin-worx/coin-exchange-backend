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
using CoinExchange.Funds.Domain.Model.WithdrawAggregate;

namespace CoinExchange.Funds.Domain.Model.Services
{
    /// <summary>
    /// Service to send events to the specific CoinClientServices to perform withdrawal after specified time interval elapse, to 
    /// create new addresses
    /// </summary>
    public interface IClientInteractionService
    {
        /// <summary>
        /// Invoked when withdrawal gets submitted successfully to the network
        /// </summary>
        event Action<Withdraw> WithdrawExecuted;

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
        /// Saves the withdraw in database and memory to be submitted after the specified time interval
        /// </summary>
        /// <param name="withdraw"></param>
        /// <returns></returns>
        bool CommitWithdraw(Withdraw withdraw);

        /// <summary>
        /// Cancels the withdraw with the given Withdraw ID
        /// </summary>
        /// <param name="withdrawId"></param>
        /// <returns></returns>
        bool CancelWithdraw(string withdrawId);

        /// <summary>
        /// Generate New Bitcoin adddress
        /// </summary>
        /// <returns></returns>
        string GenerateNewAddress(string currency);

        /// <summary>
        /// The interval after which a withdraw is submitted to the network
        /// </summary>
        double WithdrawSubmissionInterval { get; }
    }
}

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
using CoinExchange.Funds.Application.WithdrawServices.Commands;
using CoinExchange.Funds.Application.WithdrawServices.Representations;

namespace CoinExchange.Funds.Application.WithdrawServices
{
    /// <summary>
    /// Interface for dealing with operations and queries about withdraws
    /// </summary>
    public interface IWithdrawApplicationService
    {
        /// <summary>
        /// Get recent withdrawals for the given currency and account ID
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns></returns>
        List<WithdrawRepresentation> GetRecentWithdrawals(int accountId); 

        /// <summary>
        /// Get recent withdrawals for the given currency and account ID
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="currency"></param>
        /// <returns></returns>
        List<WithdrawRepresentation> GetRecentWithdrawals(int accountId, string currency); 

        /// <summary>
        /// Adds a new Bitcoin address with description
        /// </summary>
        /// <returns></returns>
        WithdrawAddressResponse AddAddress(AddAddressCommand addAddressCommand);

        /// <summary>
        /// Get the list of all the withdrawawl addresses associated with this account for this currency
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns></returns>
        List<WithdrawAddressRepresentation> GetWithdrawalAddresses(int accountId, string currency);

        /// <summary>
        /// Commits a withdraw on the user's request
        /// </summary>
        /// <param name="commitWithdrawCommand"></param>
        /// <returns></returns>
        CommitWithdrawResponse CommitWithdrawal(CommitWithdrawCommand commitWithdrawCommand);

        /// <summary>
        /// Retreives the Withdraw Limits for the given account and currency
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="currency"></param>
        /// <returns></returns>
        WithdrawLimitRepresentation GetWithdrawLimitThresholds(int accountId, string currency);

        /// <summary>
        /// Deletes the given bitcoin address from the database
        /// </summary>
        /// <param name="deleteWithdrawAddressCommand"></param>
        /// <returns></returns>
        DeleteWithdrawAddressResponse DeleteAddress(DeleteWithdrawAddressCommand deleteWithdrawAddressCommand);

        /// <summary>
        /// Cancels the withdraw from being sent to the network. Returns the boolean result and the message as a tuple
        /// </summary>
        /// <param name="cancelWithdrawCommand"></param>
        /// <returns></returns>
        CancelWithdrawResponse CancelWithdraw(CancelWithdrawCommand cancelWithdrawCommand);

        /// <summary>
        /// Get the TIer limits for Withdraw
        /// </summary>
        /// <returns></returns>
        WithdrawTierLimitRepresentation GetWithdrawTierLimits();
    }
}

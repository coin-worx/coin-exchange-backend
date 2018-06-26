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

namespace CoinExchange.Funds.Domain.Model.DepositAggregate
{
    /// <summary>
    /// Address to make a deposit to
    /// </summary>
    public class DepositAddress
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public DepositAddress()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public DepositAddress(CurrencyAggregate.Currency currency, BitcoinAddress bitcoinAddress, AddressStatus status, 
            DateTime creationDateTime, AccountId accountId)
        {
            Currency = currency;
            BitcoinAddress = bitcoinAddress;
            Status = status;
            CreationDateTime = creationDateTime;
            AccountId = accountId;
        }

        /// <summary>
        /// Database primary key
        /// </summary>
        public int Id { get; private set; }

        /// <summary>
        /// Currency
        /// </summary>
        public CurrencyAggregate.Currency Currency { get; private set; }

        /// <summary>
        /// Bitcoin Address
        /// </summary>
        public BitcoinAddress BitcoinAddress { get; set; }

        /// <summary>
        /// Status of the Address
        /// </summary>
        public AddressStatus Status { get; set; }

        /// <summary>
        /// DateTime when this address was generated
        /// </summary>
        public DateTime CreationDateTime { get; set; }

        /// <summary>
        /// AccountID
        /// </summary>
        public AccountId AccountId { get; set; }

        /// <summary>
        /// DateTime when this address is first used
        /// </summary>
        public DateTime DateUsed { get; private set; }

        /// <summary>
        /// Sets the Status set to Expired
        /// </summary>
        public void StatusExpired()
        {
            if (Status == AddressStatus.New || Status == AddressStatus.Used)
            {
                Status = AddressStatus.Expired;
            }
        }

        /// <summary>
        /// Sets the Status set to Used
        /// </summary>
        public void StatusUsed()
        {
            if (Status == AddressStatus.New)
            {
                Status = AddressStatus.Used;
                // DateTime when this address is first used
                DateUsed = DateTime.Now;
            }
        }
    }
}

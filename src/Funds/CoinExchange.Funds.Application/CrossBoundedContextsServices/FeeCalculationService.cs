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
using CoinExchange.Funds.Domain.Model.CurrencyAggregate;
using CoinExchange.Funds.Domain.Model.DepositAggregate;
using CoinExchange.Funds.Domain.Model.FeeAggregate;
using CoinExchange.Funds.Domain.Model.LedgerAggregate;

namespace CoinExchange.Funds.Application.CrossBoundedContextsServices
{
    /// <summary>
    /// Service reposnsible for matters related to calculation and retreival of fee
    /// </summary>
    public class FeeCalculationService : IFeeCalculationService
    {
        private IFeeRepository _feeRepository;
        private ILedgerRepository _ledgerRepository;
        /// <summary>
        /// Parameterized Constructor
        /// </summary>
        public FeeCalculationService(IFeeRepository feeRepository, ILedgerRepository ledgerRepository)
        {
            _feeRepository = feeRepository;
            _ledgerRepository = ledgerRepository;
        }

        #region Fee Retreival

        /// <summary>
        /// Gets the percentage fee for the given amount
        /// </summary>
        /// <returns></returns>
        public decimal GetFee(Currency baseCurrency, Currency quoteCurrency, AccountId accountId, decimal volume, decimal price)
        {
            // Get the amount traded in the last 30 days for the quote currency, because the fee is applied on the quote currency
            // only
            decimal tradesVolume = GetLast30DaysTradeVolume(quoteCurrency.Name, accountId);

            // Get the fee
            decimal fee = GetFeeInstance(baseCurrency, quoteCurrency, Math.Abs(tradesVolume));

            // Take out the percentage fee
            return Math.Abs((fee / 100) * (volume * price));
        }

        /// <summary>
        /// Get the Fee Instance from the database
        /// </summary>
        /// <param name="baseCurrency"></param>
        /// <param name="quoteCurrency"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        private decimal GetFeeInstance(Currency baseCurrency, Currency quoteCurrency, decimal amount)
        {
            string currencyPair = baseCurrency.Name + quoteCurrency.Name;
            
            List<Fee> feeList = _feeRepository.GetFeeByCurrencyPair(currencyPair);
            for (int i = 0; i < feeList.Count; i++)
            {
                // E.g., if amount == 500 && currentFee element = 1000
                if (feeList[i].Amount >= amount)
                {
                    Fee currentFee = _feeRepository.GetFeeByCurrencyAndAmount(currencyPair, feeList[i].Amount);
                    return currentFee.PercentageFee;
                }
                // If this is not the last element in the list
                if ((feeList.Count - i) != 1)
                {
                    // E.g., if amount == 1100 && currentFee = 1000 && currentFee + 1 = 2000
                    if (feeList[i].Amount < amount && feeList[i + 1].Amount > amount)
                    {
                        Fee currentFee = _feeRepository.GetFeeByCurrencyAndAmount(currencyPair, feeList[i].Amount);
                        return currentFee.PercentageFee;
                    }
                }
                // If this is the last element, that means the amount is greater than or equal to the last percentage element, 
                // so we provide the last element
                else
                {
                    if (feeList[i].Amount <= amount)
                    {
                        Fee currentFee = _feeRepository.GetFeeByCurrencyAndAmount(currencyPair, feeList[i].Amount);
                        return currentFee.PercentageFee;
                    }
                }
            }
            return 0;
        }


        #endregion Fee Retreival

        #region Private Methods

        /// <summary>
        /// Gets the volume traded by a trader for a particular currency in the last 30 days in US Dollars
        /// </summary>
        /// <returns></returns>
        private decimal GetLast30DaysTradeVolume(string quoteCurrency, AccountId accountId)
        {
            decimal tradesVolume = 0;
            IList<Ledger> allLedgers = _ledgerRepository.GetLedgerByAccountIdAndCurrency(quoteCurrency, accountId);
            if (allLedgers.Any())
            {
                foreach (var ledger in allLedgers)
                {
                    if (ledger.LedgerType == LedgerType.Trade &&
                        ledger.DateTime >= DateTime.Now.AddDays(-30) &&
                        !ledger.IsBaseCurrencyInTrade)
                    {
                        tradesVolume += ledger.Amount;
                    }
                }
            }
            return tradesVolume;
        }

        #endregion Private Methods
    }
}

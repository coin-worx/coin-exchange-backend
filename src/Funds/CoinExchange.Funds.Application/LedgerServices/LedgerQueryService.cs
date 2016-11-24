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


﻿using System.Collections.Generic;
using System.Linq;
using CoinExchange.Funds.Application.LedgerServices.Representations;
using CoinExchange.Funds.Domain.Model.DepositAggregate;
using CoinExchange.Funds.Domain.Model.LedgerAggregate;

namespace CoinExchange.Funds.Application.LedgerServices
{
    /// <summary>
    /// Service to query ledgers
    /// </summary>
    public class LedgerQueryService : ILedgerQueryService
    {
        private ILedgerRepository _ledgerRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public LedgerQueryService(ILedgerRepository ledgerRepository)
        {
            _ledgerRepository = ledgerRepository;
        }

        /// <summary>
        /// Gets all the ledgers
        /// </summary>
        /// <returns></returns>
        public IList<LedgerRepresentation> GetAllLedgers(int accountId)
        {
            IList<LedgerRepresentation> ledgerRepresentations = new List<LedgerRepresentation>();
            IList<Ledger> ledgers = _ledgerRepository.GetAllLedgers(accountId);
            foreach (var ledger in ledgers)
            {
                ledgerRepresentations.Add(new LedgerRepresentation(ledger.LedgerId, ledger.DateTime,
                    ledger.LedgerType.ToString(), ledger.Currency.Name, ledger.Amount, ledger.AmountInUsd, ledger.Fee, ledger.Balance,
                    ledger.TradeId, ledger.OrderId, ledger.WithdrawId, ledger.DepositId, ledger.AccountId.Value));
            }

            return ledgerRepresentations;
        }

        /// <summary>
        /// Get ledgers for the given currency
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="currency"></param>
        /// <returns></returns>
        public IList<LedgerRepresentation> GetLedgersForCurrency(int accountId, string currency)
        {
            IList<LedgerRepresentation> ledgerRepresentations = new List<LedgerRepresentation>();
            IList<Ledger> ledgers = _ledgerRepository.GetLedgerByAccountIdAndCurrency(currency, new AccountId(accountId));
            foreach (var ledger in ledgers)
            {
                ledgerRepresentations.Add(new LedgerRepresentation(ledger.LedgerId, ledger.DateTime,
                    ledger.LedgerType.ToString(), ledger.Currency.Name, ledger.Amount, ledger.AmountInUsd, ledger.Fee, ledger.Balance,
                    ledger.TradeId, ledger.OrderId, ledger.WithdrawId, ledger.DepositId, ledger.AccountId.Value));
            }

            return ledgerRepresentations;
        }

        /// <summary>
        /// Get the details for the given ledger ID
        /// </summary>
        /// <param name="ledgerId"></param>
        /// <returns></returns>
        public LedgerRepresentation GetLedgerDetails(string ledgerId)
        {
            Ledger ledger = _ledgerRepository.GetLedgerByLedgerId(ledgerId);
            return new LedgerRepresentation(ledger.LedgerId, ledger.DateTime, ledger.LedgerType.ToString(), ledger.Currency.Name,
                ledger.Amount, ledger.AmountInUsd, ledger.Fee, ledger.Balance, ledger.TradeId, ledger.OrderId, ledger.WithdrawId,
                ledger.DepositId, ledger.AccountId.Value);
        }
    }
}

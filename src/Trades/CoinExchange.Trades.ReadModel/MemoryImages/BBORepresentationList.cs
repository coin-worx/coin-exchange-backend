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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Trades.Domain.Model.OrderAggregate;
using CoinExchange.Trades.Domain.Model.OrderMatchingEngine;

namespace CoinExchange.Trades.ReadModel.MemoryImages
{
    /// <summary>
    /// BBORepresentationList
    /// </summary>
    public class BBORepresentationList : IEnumerable<BBORepresentation>
    {
        // Get the Current Logger
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger
        (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Each slot contains the BBO representation, containg best prices, best volumes and their order counts
        /// </summary>
        private List<BBORepresentation> _bboRepresentations = null;

        /// <summary>
        /// Default Constructor
        /// </summary>
        public BBORepresentationList()
        {
            _bboRepresentations = new List<BBORepresentation>();
        }

        /// <summary>
        /// Adds the best bid and best offer
        /// </summary>
        /// <returns></returns>
        public bool AddBBO(string currencyPair, DepthLevel bestBid, DepthLevel bestAsk)
        {
            var bboRepresentation = Contains(currencyPair);
            if (bboRepresentation != null)
            {
                _bboRepresentations.Remove(bboRepresentation);
            }
            if ((bestBid.Price != null && bestBid.AggregatedVolume != null) || (bestAsk.Price != null && bestAsk.AggregatedVolume != null))
            {
                _bboRepresentations.Add(new BBORepresentation(currencyPair, 
                                                        // If price or volume values are null, just assign 0 as the best price and/or volume
                                                        bestBid.Price != null ? bestBid.Price.Value : 0,
                                                        bestBid.AggregatedVolume != null ? bestBid.AggregatedVolume.Value : 0,
                                                        bestBid.OrderCount, 
                                                        bestAsk.Price != null ? bestAsk.Price.Value : 0,
                                                        bestAsk.AggregatedVolume != null ? bestAsk.AggregatedVolume.Value : 0, 
                                                        bestAsk.OrderCount));
                return true;
            }
            return false;
        }

        /// <summary>
        /// Contains the BBO Representation
        /// </summary>
        /// <param name="currencyPair"></param>
        /// <returns></returns>
        public BBORepresentation Contains(string currencyPair)
        {
            foreach (BBORepresentation bboRepresentation in _bboRepresentations)
            {
                if (bboRepresentation.CurrencyPair == currencyPair)
                {
                    return bboRepresentation;
                }
            }
            return null;
        }

        #region Implementation of IEnumerable

        /// <summary>
        /// GetEnumerator(specific)
        /// </summary>
        /// <returns></returns>
        IEnumerator<BBORepresentation> IEnumerable<BBORepresentation>.GetEnumerator()
        {
            foreach (BBORepresentation bboRepresentation in _bboRepresentations)
            {
                if (bboRepresentation == null)
                {
                    break;
                }

                // Return the current element and then on next function call 
                // resume from next element rather than starting all over again;
                yield return bboRepresentation;
            }
        }

        /// <summary>
        /// GetEnumerator(generic)
        /// </summary>
        /// <returns></returns>
        public IEnumerator GetEnumerator()
        {
            foreach (BBORepresentation bboRepresentation in _bboRepresentations)
            {
                if (bboRepresentation == null)
                {
                    break;
                }

                // Return the current element and then on next function call 
                // resume from next element rather than starting all over again;
                yield return bboRepresentation;
            }
        }

        #endregion
    }
}

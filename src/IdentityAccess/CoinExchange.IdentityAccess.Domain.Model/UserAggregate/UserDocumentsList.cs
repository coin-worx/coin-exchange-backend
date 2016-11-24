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

namespace CoinExchange.IdentityAccess.Domain.Model.UserAggregate
{
    /// <summary>
    /// User Document List
    /// </summary>
    public class UserDocumentsList : IEnumerable<UserDocument>
    {
        private List<UserDocument> _userDocumentList = new List<UserDocument>();

        /// <summary>
        /// Add an element
        /// </summary>
        /// <returns></returns>
        internal bool AddTierStatus(UserDocument userDocument)
        {
            _userDocumentList.Add(userDocument);
            return true;
        }

        /// <summary>
        /// Remove an element
        /// </summary>
        /// <param name="userDocument"></param>
        /// <returns></returns>
        internal bool RemoveTierStatus(UserDocument userDocument)
        {
            _userDocumentList.Remove(userDocument);
            return true;
        }

        /// <summary>
        /// GetEnumerator - Specific
        /// </summary>
        /// <returns></returns>
        public IEnumerator<UserDocument> GetEnumerator()
        {
            foreach (var userDocument in _userDocumentList)
            {
                // Lets check for end of list (its bad code since we used arrays)
                if (userDocument == null)
                {
                    break;
                }

                // Return the current element and then on next function call 
                // resume from next element rather than starting all over again;
                yield return userDocument;
            }
        }

        /// <summary>
        /// GetEnumerator - Generic
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}

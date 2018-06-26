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

namespace CoinExchange.Trades.ReadModel.MemoryImages
{
    /// <summary>
    /// Contains a list of Depth level representations, represnts one currency pair
    /// </summary>
    public class DepthRepresentation : IDictionary<string, DepthLevelRepresentationList>
    {
        // Get the Current Logger
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger
        (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private Dictionary<string, DepthLevelRepresentationList> _depths = new Dictionary<string, DepthLevelRepresentationList>(); 

        #region Implementation of IEnumerable

        public IEnumerator<KeyValuePair<string, DepthLevelRepresentationList>> GetEnumerator()
        {
            foreach (KeyValuePair<string,DepthLevelRepresentationList> keyValuePair in _depths)
            {
                // Lets check for end of list (its bad code since we used arrays)
                if (keyValuePair.Key == null || keyValuePair.Value == null)
                {
                    break;
                }

                // Return the current element and then on next function call 
                // resume from next element rather than starting all over again;
                yield return keyValuePair;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        // NOTE: The default public methods for the dicionary for addition and removal are not implmented on purpose, because
        // we will implement our own methods with internal access so anyone outside the assembly cannot access them
        #region Implementation of ICollection<KeyValuePair<string,DepthLevelRepresentationList>>

        public void Add(KeyValuePair<string, DepthLevelRepresentationList> item)
        {
            throw new NotImplementedException("The Add() method just called cannot be used. Only classes internal to the " +
                                              "assembly can add items to this dictionary.");
        }

        public void Clear()
        {
            throw new NotImplementedException("Cannot clear the dictionary from outside the assembly. Only methods within" +
                                              " the assembly of this dictionary can clear it.");
        }

        /// <summary>
        /// Clears the dictionary
        /// </summary>
        internal void ClearDepth()
        {
            _depths.Clear();
        }

        /// <summary>
        /// Contains
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Contains(KeyValuePair<string, DepthLevelRepresentationList> item)
        {
            return _depths.Contains(item);
        }

        /// <summary>
        /// CopyTo
        /// </summary>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
        public void CopyTo(KeyValuePair<string, DepthLevelRepresentationList>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Remove
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Remove(KeyValuePair<string, DepthLevelRepresentationList> item)
        {
            throw new NotImplementedException("Cannot remove the depth from outside the assembly of this dictionary. Only " +
                                              "classes within the assembly can remove from this dictionary.");
        }

        public int Count { get { return _depths.Count; } private set { } }
        public bool IsReadOnly { get { return false; } private set{} }

        #endregion

        #region Implementation of IDictionary<string,DepthLevelRepresentationList>
        
        /// <summary>
        /// Conatains Key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool ContainsKey(string key)
        {
            return _depths.ContainsKey(key);
        }

        /// <summary>
        /// Add
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Add(string key, DepthLevelRepresentationList value)
        {
            throw new NotImplementedException("The Add() method just called cannot be used. Only classes internal to the " +
                                              "assembly can add items to this dictionary.");
        }

        /// <summary>
        /// Add Depth. Depth can be added to this collection using only this method from inside this assembly
        /// </summary>
        /// <param name="currencypair"></param>
        /// <param name="depthLevelRepresentation"></param>
        internal void AddDepth(string currencypair, DepthLevelRepresentationList depthLevelRepresentation)
        {
            _depths.Add(currencypair, depthLevelRepresentation);
        }

        /// <summary>
        /// Remove(Method not applicable to this collection)
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool Remove(string key)
        {
            throw new NotImplementedException("Cannot remove the depth from outside the assembly of this dictionary. Only " +
                                              "classes within the assembly can remove from this dictionary.");
        }

        /// <summary>
        /// TrygetValue
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryGetValue(string key, out DepthLevelRepresentationList value)
        {
            throw new NotImplementedException("TryGetValue cannot be called on DepthRepresentation. Try 'TryGetDepth' from inside" +
                                              " the assembly of DepthRepresentation.");
        }

        internal bool TryGetDepth(string key, out  DepthLevelRepresentationList value)
        {
            return _depths.TryGetValue(key, out value);
        }

        /// <summary>
        /// Assignment
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public DepthLevelRepresentationList this[string key]
        {
            get { return _depths[key]; }
            set { throw new NotImplementedException("Cannot set a value to this dictionary"); }
        }

        public ICollection<string> Keys { get { return _depths.Keys; } private set { } }
        public ICollection<DepthLevelRepresentationList> Values { get { return _depths.Values; } private set{} }

        #endregion

        /// <summary>
        /// Sets the given value against the given key as we nned to provide this setting fuctionality internally
        /// </summary>
        /// <param name="key"></param>
        /// <param name="depthLevels"></param>
        /// <returns></returns>
        internal bool SetValue(string key, DepthLevelRepresentationList depthLevels)
        {
            _depths[key] = depthLevels;
            return true;
        }
    }
}

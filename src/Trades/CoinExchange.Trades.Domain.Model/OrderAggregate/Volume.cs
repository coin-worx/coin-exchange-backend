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
using CoinExchange.Common.Domain.Model;

namespace CoinExchange.Trades.Domain.Model.OrderAggregate
{
    /// <summary>
    /// order volume value object
    /// </summary>
    [Serializable]
    public class Volume : IComparable<Volume>
    {
        private decimal _value;
        public decimal Value
        {
            get { return _value; }
            private set
            {
                //AssertionConcern.AssertGreaterThanZero(value,"Volume must be greater than 0");
                _value = value;
            }
        }

        public Volume(decimal value)
        {
            Value = value;
        }

        public bool IsGreaterThan(Volume volume)
        {
            if (volume == null)
            {
                return false;
            }
            return _value > volume._value;
        }

        public int CompareTo(Volume volume)
        {
            if (this.Value > volume.Value) return -1;
            if (this.Value == volume.Value) return 0;
            return 1;
        }

        public override string ToString()
        {
            return "Order Volume=" + _value;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            return this._value == (obj as Volume)._value;
        }

        /// <summary>
        /// += operator
        /// </summary>
        /// <param name="volume1"></param>
        /// <param name="volume2"></param>
        /// <returns></returns>
        public static Volume operator +(Volume volume1, Volume volume2)
        {
            if (volume1 == null || volume2 == null)
            {
                return null;
            }
            return new Volume(volume1.Value + volume2.Value);
        }

        /// <summary>
        /// += operator
        /// </summary>
        /// <param name="volume1"></param>
        /// <param name="volume2"></param>
        /// <returns></returns>
        public static Volume operator -(Volume volume1, Volume volume2)
        {
            if (volume1 == null || volume2 == null)
            {
                return null;
            }
            return new Volume(volume1.Value - volume2.Value);
        }

        public static bool operator >(Volume x, Volume y)
        {
            return x.Value > y.Value;
        }

        public static bool operator <(Volume x, Volume y)
        {
            return x.Value < y.Value;
        }

        public static bool operator >=(Volume x, Volume y)
        {
            return x.Value >= y.Value;
        }

        public static bool operator <=(Volume x, Volume y)
        {
            return x.Value <= y.Value;
        }
    }
}

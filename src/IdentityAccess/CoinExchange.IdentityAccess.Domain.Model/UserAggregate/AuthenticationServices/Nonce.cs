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
using System.Collections.Concurrent;
using System.Security.Cryptography;

namespace CoinExchange.IdentityAccess.Domain.Model.UserAggregate.AuthenticationServices
{
    /// <summary>
    /// Serves the purpose of maintaing and genrating nonces
    /// </summary>
    class Nonce
    {
        private static ConcurrentDictionary<string, Tuple<int, DateTime>>
          nonces = new ConcurrentDictionary<string, Tuple<int, DateTime>>();

        public static string Generate()
        {
            byte[] bytes = new byte[16];

            using (var rngProvider = new RNGCryptoServiceProvider())
            {
                rngProvider.GetBytes(bytes);
            }

            string nonce = bytes.ToMD5Hash();

            nonces.TryAdd(nonce, new Tuple<int, DateTime>(0, DateTime.Now.AddMinutes(10)));

            return nonce;
        }

        public static bool IsValid(string nonce, string nonceCount)
        {
            Tuple<int, DateTime> cachedNonce = null;
            nonces.TryGetValue(nonce, out cachedNonce);

            if (cachedNonce != null) // nonce is found
            {
                // nonce count is greater than the one in record
                if (Int32.Parse(nonceCount) > cachedNonce.Item1)
                {
                    // nonce has not expired yet
                    if (cachedNonce.Item2 > DateTime.Now)
                    {
                        // update the dictionary to reflect the nonce count just received in this request
                        nonces[nonce] = new Tuple<int, DateTime>(Int32.Parse(nonceCount),cachedNonce.Item2);

                        // Every thing looks ok - server nonce is fresh and nonce count seems to be 
                        // incremented. Does not look like replay.
                        return true;
                    }
                    else
                    {
                        Tuple<int, DateTime> tuple;
                        nonces.TryRemove(nonce, out tuple);
                    }
                }
            }

            return false;
        }
    }
}

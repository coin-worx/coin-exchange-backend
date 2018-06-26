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
using Newtonsoft.Json;

namespace CoinExchange.Client.Tests
{
    public class AccessControl
    {
        private IdentityAccessClient _client;
        private string _userName;
        private string _password;

        public AccessControl(IdentityAccessClient client, string password, string userName)
        {
            _client = client;
            this._password = password;
            this._userName = userName;
        }

        public string CreateAndActivateUser(string username,string password,string email)
        {
            string activationcode = _client.SignUp(email, username, password, "pakistan",
                TimeZone.CurrentTimeZone, "");
            string code = JsonConvert.DeserializeObject<string>(activationcode);
            Console.WriteLine(_client.ActivateUser(username, password, code));
            return "Activated";
        }

        public string Login(string username,string password)
        {
            string login = _client.Login(username, password);
            Console.WriteLine(login);
            dynamic keys = JsonConvert.DeserializeObject<dynamic>(login);
            _client.key = keys.ApiKey;
            _client.secretkey = keys.SecretKey;
            return "";
        }

        public PermissionRepresentation[] ListPermissions()
        {
            return JsonConvert.DeserializeObject<PermissionRepresentation[]>(_client.ListPermissions());
        }

        public string GetSecurityPairs()
        {
            return _client.KeyPairList();
        }

        public string CreateSecurityKeyPair(string keydescription,PermissionRepresentation[] permissions)
        {
            return _client.CreateKey(keydescription,permissions);
        }
    }
}

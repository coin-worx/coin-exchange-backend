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
using System.Configuration;
using System.Threading;
using CoinExchange.Client.Tests;
using CoinExchange.Common.Tests;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CoinExchange.Client.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            string baseUrl = "http://rockblanc.cloudapp.net/test/v1";
            baseUrl = "http://localhost:51780/v1";
            //ApiClient client = new ApiClient(baseUrl);
            //System.Console.WriteLine(client.QueryTrades("6e8b5195-0e7f-402f-87e7-80eb92a96c85"));
            //Scenario1(client);
            //ScenarioResults(client);
            //System.Console.WriteLine(client.GetTradeHistory("",""));
            Login(baseUrl);
            //Scenario1(client);

            AllBoundedContextsIntegrationTests allBoundedContexts = new AllBoundedContextsIntegrationTests();
            //allBoundedContexts.Initialization();
            System.Console.ReadKey();
        }

        /// <summary>
        /// Testing scenario 1
        /// </summary>
        private static void Scenario1(ApiClient client)
        {
            string currencyPair = "XBTUSD";
            //Create orders
            System.Console.WriteLine(client.CreateOrder(currencyPair, "limit", "buy", 10, 250));
            System.Console.WriteLine(client.CreateOrder(currencyPair, "limit", "sell", 5, 252));
            System.Console.WriteLine(client.CreateOrder(currencyPair, "market", "buy", 3));
            System.Console.WriteLine(client.CreateOrder(currencyPair, "limit", "buy", 2, 253));
            System.Console.WriteLine(client.CreateOrder(currencyPair, "market", "sell", 5));
            System.Console.WriteLine(client.CreateOrder(currencyPair, "limit", "buy", 2, 250));
            Thread.Sleep(5000);
            ScenarioResults(client);
        }

        /// <summary>
        /// Testing Scenario 2
        /// </summary>
        /// <param name="client"></param>
        private static void Scenario2(ApiClient client)
        {
            string currecyPair = "XBTUSD";
            JObject joe = JObject.Parse(client.CreateOrder(currecyPair, "market", "buy", 10));
            string orderId1 = joe.Property("OrderId").Value.ToString();
            string orderId2 = JObject.Parse(client.CreateOrder(currecyPair, "limit", "sell", 5, 252)).Property("OrderId").Value.ToString();
            string orderId3 = JObject.Parse(client.CreateOrder(currecyPair, "limit", "buy", 8, 245)).Property("OrderId").Value.ToString();
            string orderId4 = JObject.Parse(client.CreateOrder(currecyPair, "limit", "buy", 7, 250)).Property("OrderId").Value.ToString();
            string orderId5 = JObject.Parse(client.CreateOrder(currecyPair, "market", "buy", 3)).Property("OrderId").Value.ToString();
            Thread.Sleep(2000);
            System.Console.WriteLine(client.CancelOrder(orderId2));
            string orderId6 = JObject.Parse(client.CreateOrder(currecyPair, "market", "buy", 10)).Property("OrderId").Value.ToString();
            string orderId7 = JObject.Parse(client.CreateOrder(currecyPair, "market", "sell", 5)).Property("OrderId").Value.ToString();
            System.Console.WriteLine(client.CancelOrder(orderId2));
            string orderId9 = client.CreateOrder(currecyPair, "market", "sell", 4);
            string orderId10 = client.CreateOrder(currecyPair, "market", "sell", 5);
            Thread.Sleep(5000);
            ScenarioResults(client);
        }

        /// <summary>
        /// Testing scenario 3
        /// </summary>
        /// <param name="client"></param>
        private static void Scenario3(ApiClient client)
        {
            string currecyPair = "XBTUSD";
            JObject joe = JObject.Parse(client.CreateOrder(currecyPair, "limit", "sell", 5, 252));
            string orderId1 = joe.Property("OrderId").Value.ToString();
            string orderId2 = JObject.Parse(client.CreateOrder(currecyPair, "limit", "buy", 8, 245)).Property("OrderId").Value.ToString();
            string orderId3 = JObject.Parse(client.CreateOrder(currecyPair, "limit", "buy", 7, 250)).Property("OrderId").Value.ToString();
            string orderId4 = JObject.Parse(client.CreateOrder(currecyPair, "limit", "buy", 3, 250)).Property("OrderId").Value.ToString();
            string orderId5 = JObject.Parse(client.CreateOrder(currecyPair, "limit", "sell", 5, 253)).Property("OrderId").Value.ToString();
            string orderId6 = JObject.Parse(client.CreateOrder(currecyPair, "limit", "buy", 8, 240)).Property("OrderId").Value.ToString();
            string orderId7 = JObject.Parse(client.CreateOrder(currecyPair, "limit", "buy", 7, 245)).Property("OrderId").Value.ToString();
            string orderId8 = JObject.Parse(client.CreateOrder(currecyPair, "limit", "buy", 3, 247)).Property("OrderId").Value.ToString();
            Thread.Sleep(5000);
            System.Console.WriteLine(client.CancelOrder(orderId6));
            System.Console.WriteLine(client.CancelOrder(orderId1));
            string orderId9 = client.CreateOrder(currecyPair, "market", "sell", 9);
            System.Console.WriteLine(client.CancelOrder(orderId8));
            System.Console.WriteLine(client.CancelOrder(orderId7));
            System.Console.WriteLine(client.CancelOrder(orderId6));
            System.Console.WriteLine(client.CancelOrder(orderId5));
            System.Console.WriteLine(client.CancelOrder(orderId4));
            System.Console.WriteLine(client.CancelOrder(orderId3));
            System.Console.WriteLine(client.CancelOrder(orderId2));
            System.Console.WriteLine(client.CancelOrder(orderId1));
            Thread.Sleep(5000);
            ScenarioResults(client);
        }

        private static void ScenarioResults(ApiClient client)
        {
            System.Console.WriteLine("------RESULTS------");
            System.Console.WriteLine("------OPEN ORDERS------");
            System.Console.WriteLine(client.QueryOpenOrdersParams(true, ""));
            System.Console.WriteLine("------CLOSED ORDERS------");
            System.Console.WriteLine(client.QueryClosedOrdersParams(true, "", ""));
            System.Console.WriteLine("------TRADES------");
            System.Console.WriteLine(client.GetTradeHistory("", ""));
        }

        private static void OrderBookGenerator(ApiClient client)
        {
            string currencyPair = "XBTUSD";
            //Create orders
            System.Console.WriteLine(client.CreateOrder(currencyPair, "limit", "buy", 10, 250));
            Thread.Sleep(2000);
            System.Console.WriteLine(client.CreateOrder(currencyPair, "limit", "sell", 5, 252));
            Thread.Sleep(2000);
            System.Console.WriteLine(client.CreateOrder(currencyPair, "limit", "buy", 3, 250));
            Thread.Sleep(2000);
            System.Console.WriteLine(client.CreateOrder(currencyPair, "limit", "sell", 2, 253));
            Thread.Sleep(2000);
            System.Console.WriteLine(client.CreateOrder(currencyPair, "limit", "sell", 5,254));
            Thread.Sleep(2000);
           // System.Console.WriteLine(client.CreateOrder(currencyPair, "limit", "buy", 2, 249));
           // Thread.Sleep(5000);
            ScenarioResults(client);
        }

        private static void Login(string baseUrl)
        {
            IdentityAccessClient client=new IdentityAccessClient(baseUrl);
            AccessControl control=new AccessControl(client,"pa$$word","bruce");
            //control.CreateAndActivateUser("user1", "123", "jonsnow@user.com");
            control.Login("user1", "123");
            //Scenario1(client);
            //OrderBookGenerator(client);
            System.Console.WriteLine(client.GetBalances());
            //ScenarioResults(client);
            //System.Console.WriteLine(control.GetSecurityPairs());
            //PermissionRepresentation[] rep = control.ListPermissions();
            //rep[0].Allowed = true;
            //System.Console.WriteLine(control.CreateSecurityKeyPair("#1",rep));
            client.Logout();
        }

        
    }
}

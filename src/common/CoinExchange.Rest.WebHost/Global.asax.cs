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
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using CoinExchange.Common.Domain.Model;
using CoinExchange.Rest.WebHost.App_Start;
using CoinExchange.Trades.Domain.Model.CurrencyPairAggregate;
using CoinExchange.Trades.Domain.Model.OrderAggregate;
using CoinExchange.Trades.Domain.Model.OrderMatchingEngine;
using CoinExchange.Trades.Domain.Model.Services;
using CoinExchange.Trades.Infrastructure.Persistence.RavenDb;
using Common.Logging;
using Disruptor;
using Spring.Context;
using Spring.Context.Support;
//using Spring.Web.Mvc;

namespace CoinExchange.Rest.WebHost
{
    public class WebApiApplication:HttpApplication//: SpringMvcApplication
    {
        protected void Application_Start()
        {
            log4net.Config.XmlConfigurator.Configure();
            //IApplicationContext applicationContext = ContextRegistry.GetContext();
            
            //ControllerBuilder.Current.SetControllerFactory(new SpringControllerFactory());
           // RegisterRoutes(RouteTable.Routes);
            GlobalConfiguration.Configure(WebApiConfig.Register);
            //  LogManager.GetCurrentClassLogger().Info("Application started...");
            //IControllerFactory factory = new SpringControllerFactory();
            //ControllerBuilder.Current.SetControllerFactory(factory);
            //DependencyResolver.SetResolver(new SpringDependencyResolver(ContextRegistry.GetContext()));
            //DependencyResolver.SetResolver(new SpringDependencyResolver(ContextRegistry.GetContext()));
            InitiliazeApplication();
        }

        //protected override System.Web.Http.Dependencies.IDependencyResolver BuildWebApiDependencyResolver()
        //{
        //    //get the 'default' resolver, populated from the 'main' config metadata
        //    var resolver = base.BuildWebApiDependencyResolver();

        //    //check if its castable to a SpringWebApiDependencyResolver
        //    var springResolver = resolver as SpringWebApiDependencyResolver;

        //    //return the fully-configured resolver
        //    return springResolver;
        //}

        /// <summary>
        /// Method for initializaing the single threaded application parts
        /// </summary>
        private void InitiliazeApplication()
        {
            InputDisruptorPublisher.Shutdown();
            OutputDisruptor.ShutDown();

            //NOTE: Event Store not working as the RavenDB server has been updated with different REST calls
            /*IEventStore inputEventStore = new RavenNEventStore(Constants.INPUT_EVENT_STORE);
            IEventStore outputEventStore = new RavenNEventStore(Constants.OUTPUT_EVENT_STORE);
            Journaler inputJournaler = new Journaler(inputEventStore);
            Journaler outputJournaler = new Journaler(outputEventStore);
            ExchangeEssentialsList exchangeEssentialsList=outputEventStore.LoadLastSnapshot();*/

            Journaler inputJournaler = null;
            Journaler outputJournaler = null;

            ICurrencyPairRepository currencyPairRepository = (ICurrencyPairRepository) ContextRegistry.GetContext()["CurrencyPairRepository"];
            IList<CurrencyPair> tradeableCurrencyPairs = currencyPairRepository.GetTradeableCurrencyPairs();
            Exchange exchange;
            /*if (exchangeEssentialsList != null)
            {
                //means snapshot found so initialize the exchange
                exchange = new Exchange(tradeableCurrencyPairs, exchangeEssentialsList);
                InputDisruptorPublisher.InitializeDisruptor(new IEventHandler<InputPayload>[] {exchange, inputJournaler});
                OutputDisruptor.InitializeDisruptor(new IEventHandler<byte[]>[] {outputJournaler});
                exchange.InitializeExchangeAfterSnaphot();
                LimitOrderBookReplayService service = new LimitOrderBookReplayService();
                service.ReplayOrderBooks(exchange, outputJournaler);
                exchange.EnableSnaphots(Constants.SnaphsortInterval);
            }
            else*/
            {
                //no snapshot found
                exchange = new Exchange(tradeableCurrencyPairs);
                InputDisruptorPublisher.InitializeDisruptor(new IEventHandler<InputPayload>[] { exchange/*, null*/ });
                // OutputDisruptor.InitializeDisruptor(new IEventHandler<byte[]>[] { null });
               // check if there are events to replay
                LimitOrderBookReplayService service = new LimitOrderBookReplayService();
                service.ReplayOrderBooks(exchange, null);
                exchange.EnableSnaphots(Constants.SnaphsortInterval);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using CoinExchange.Common.Domain.Model;
using CoinExchange.Rest.WebHost.App_Start;
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
            IEventStore inputEventStore = new RavenNEventStore(Constants.INPUT_EVENT_STORE);
            IEventStore outputEventStore = new RavenNEventStore(Constants.OUTPUT_EVENT_STORE);
            Journaler inputJournaler = new Journaler(inputEventStore);
            Journaler outputJournaler = new Journaler(outputEventStore);
            ExchangeEssentialsList exchangeEssentialsList=outputEventStore.LoadLastSnapshot();
            Exchange exchange;
            if (exchangeEssentialsList != null)
            {
                //means snapshot found so initialize the exchange
                exchange = new Exchange(exchangeEssentialsList);
                InputDisruptorPublisher.InitializeDisruptor(new IEventHandler<InputPayload>[] {exchange, inputJournaler});
                OutputDisruptor.InitializeDisruptor(new IEventHandler<byte[]>[] {outputJournaler});
                exchange.InitializeExchangeAfterSnaphot();
                LimitOrderBookReplayService service = new LimitOrderBookReplayService();
                service.ReplayOrderBooks(exchange, outputJournaler);
                exchange.EnableSnaphots(Constants.SnaphsortInterval);
            }
            else
            {
                //no snapshot found
                exchange = new Exchange();
                InputDisruptorPublisher.InitializeDisruptor(new IEventHandler<InputPayload>[] { exchange, inputJournaler });
                OutputDisruptor.InitializeDisruptor(new IEventHandler<byte[]>[] { outputJournaler });
               // check if there are events to replay
                LimitOrderBookReplayService service = new LimitOrderBookReplayService();
                service.ReplayOrderBooks(exchange, outputJournaler);
                exchange.EnableSnaphots(Constants.SnaphsortInterval);
            }
        }
    }
}

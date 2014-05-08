using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;
using CoinExchange.Trades.Application.OrderServices.Representation;
using CoinExchange.Trades.Domain.Model.OrderAggregate;
using CoinExchange.Trades.Domain.Model.OrderMatchingEngine;
using CoinExchange.Trades.Port.Adapter.Rest.DTOs.Order;
using CoinExchange.Trades.Port.Adapter.Rest.Resources;
using CoinExchange.Trades.ReadModel.MemoryImages;
using Disruptor;
using NUnit.Framework;
using Spring.Context;
using Spring.Context.Support;

namespace CoinExchange.Trades.Port.Adapter.Rest.IntegrationTests
{
    /// <summary>
    /// Tests the services in the OrderController
    /// </summary>
    [TestFixture]
    class OrderControllerTests
    {
        [Test]
        [Category("Integration")]
        public void CreateOrderTest()
        {
            // Get the context
            IApplicationContext applicationContext = ContextRegistry.GetContext();
            Exchange exchange = new Exchange();
            InputDisruptorPublisher.InitializeDisruptor(new IEventHandler<InputPayload>[]{exchange});

            // Get the instance through Spring configuration
            OrderController orderController = (OrderController)applicationContext["OrderController"];

            IHttpActionResult httpActionResult = orderController.CreateOrder(new CreateOrderParam()
                                                                                 {
                                                                                     Pair = "BTCUSD", Price = 491, Volume = 100, Side = "buy", Type = "limit"
                                                                                 });
            ManualResetEvent manualReset = new ManualResetEvent(false);
            manualReset.WaitOne(7000);

            OkNegotiatedContentResult<NewOrderRepresentation> okResponseMessage =
                (OkNegotiatedContentResult<NewOrderRepresentation>)httpActionResult;
            Assert.IsNotNull(okResponseMessage.Content);
        }
    }
}

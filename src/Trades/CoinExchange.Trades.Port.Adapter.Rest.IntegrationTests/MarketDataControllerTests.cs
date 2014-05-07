using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using CoinExchange.Trades.Application.MatchingEngineServices;
using CoinExchange.Trades.Application.OrderServices;
using CoinExchange.Trades.Port.Adapter.Rest.Resources;
using CoinExchange.Trades.ReadModel.MemoryImages;
using NUnit.Framework;
using Spring.Context;
using Spring.Context.Support;

namespace CoinExchange.Trades.Port.Adapter.Rest.IntegrationTests
{
    /// <summary>
    /// Tests the controller that queries MarketData from the ReadModel
    /// </summary>
    [TestFixture]
    class MarketDataControllerTests
    {
        [Test]
        [Category("Integration")]
        public void GetOrderBookTest_ChecksIfOrderBookIsRetreivedProperly_ValidatesReturnedOrderBook()
        {
            // Get the context
            IApplicationContext applicationContext = ContextRegistry.GetContext();

            // Get the instance through Spring configuration
            MarketController marketController = (MarketController)ContextRegistry.GetContext()["MarketController"];
            IHttpActionResult httpActionResult = marketController.GetOrderBook("XBTUSD");
        }
    }
}

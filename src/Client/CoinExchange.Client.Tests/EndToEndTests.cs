using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CoinExchange.Common.Tests;
using NUnit.Framework;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CoinExchange.Client.Tests
{
    [TestFixture]
    public class EndToEndTests
    {
        private DatabaseUtility _databaseUtility;
        
        [SetUp]
        public new void SetUp()
        {
            var connection = ConfigurationManager.ConnectionStrings["MySql"].ToString();
            _databaseUtility = new DatabaseUtility(connection);
            _databaseUtility.Create();
            _databaseUtility.Populate();
        }

        [TearDown]
        public new void TearDown()
        {
            _databaseUtility.Create();
        }

        [Test]
        [Category("Integration")]
        public void Scenario1Test_TestsScenario1AndItsOutcome_VerifiesThroughMarketDataOrderAndTradesResults()
        {
            ApiClient apiClient = GetClient();
            Scenario1OrderCreation(apiClient);

            // ------------------------------- Order Book ------------------------------
            dynamic orderBook = JsonConvert.DeserializeObject<dynamic>(apiClient.GetOrderBook("XBTUSD"));

            // Item1 = Bid Book, Item1[i].Item1 = Volume of 'i' Bid, Item1[i].Item2 = Price of 'i' Bid
            Assert.AreEqual(5, orderBook.m_Item1[0].m_Item1.Value);
            Assert.AreEqual(250, orderBook.m_Item1[0].m_Item2.Value);
            Assert.AreEqual(2, orderBook.m_Item1[1].m_Item1.Value);
            Assert.AreEqual(250, orderBook.m_Item1[1].m_Item2.Value);

            // Item2 = Ask Book, Item2[i].Item1 = Volume of Ask at index 'i', Item2[i].Item2 = Price of Ask at index 'i'
            Assert.AreEqual(0, orderBook.m_Item2.Count);
            // ------------------------------- Order Book ------------------------------
            dynamic depth = JsonConvert.DeserializeObject<dynamic>(apiClient.GetDepth("XBTUSD"));

            // Item1 = Bid Book, Item1.Item1 = Aggregated Volume, Item1.Item2 = Price, Item1.Item3 = Number of Orders
            Assert.AreEqual(7, depth.m_Item1[0].m_Item1.Value);
            Assert.AreEqual(250, depth.m_Item1[0].m_Item2.Value);
            Assert.AreEqual(2, depth.m_Item1[0].m_Item3.Value);
            Assert.IsNull(depth.m_Item1[1].Value);
            Assert.IsNull(depth.m_Item1[2].Value);
            Assert.IsNull(depth.m_Item1[3].Value);
            Assert.IsNull(depth.m_Item1[4].Value);

            // Item2 = Bid Book, Item2.Item1 = Aggregated Volume, Item2.Item2 = Price, Item2.Item3 = Number of Orders
            Assert.IsNull(depth.m_Item2[0].Value);
            Assert.IsNull(depth.m_Item2[1].Value);
            Assert.IsNull(depth.m_Item2[2].Value);
            Assert.IsNull(depth.m_Item2[3].Value);
            Assert.IsNull(depth.m_Item2[4].Value);

            dynamic openOrders = GetOpenOrders(apiClient);
            GetOpenOrders(apiClient);
            //------------------- Open Orders -------------------------            
            Assert.AreEqual(2, openOrders.Count);
            // First Open Order
            Assert.AreEqual(250, openOrders[0].Price.Value);
            Assert.AreEqual(10, openOrders[0].Volume.Value);
            Assert.AreEqual(5, openOrders[0].VolumeExecuted.Value);
            Assert.AreEqual(5, openOrders[0].OpenQuantity.Value);
            Assert.AreEqual("Limit", openOrders[0].OrderType.Value);
            Assert.AreEqual("Buy", openOrders[0].OrderSide.Value);
            Assert.AreEqual("PartiallyFilled", openOrders[0].Status.Value);

            // Second Open Order
            Assert.AreEqual(250, openOrders[1].Price.Value);
            Assert.AreEqual(2, openOrders[1].Volume.Value);
            Assert.AreEqual(0, openOrders[1].VolumeExecuted.Value);
            Assert.AreEqual(2, openOrders[1].OpenQuantity.Value);
            Assert.AreEqual("Limit", openOrders[1].OrderType.Value);
            Assert.AreEqual("Buy", openOrders[1].OrderSide.Value);
            Assert.AreEqual("Accepted", openOrders[1].Status.Value);
            //------------------- Open Orders -------------------------

            //------------------- Closed Orders -------------------------
            dynamic closedOrders = GetClosedOrders(apiClient);
            Assert.AreEqual(252, closedOrders[0].Price.Value);
            Assert.AreEqual(5, closedOrders[0].Volume.Value);
            Assert.AreEqual(5, closedOrders[0].VolumeExecuted.Value);
            Assert.AreEqual(0, closedOrders[0].OpenQuantity.Value);
            Assert.AreEqual("Limit", closedOrders[0].OrderType.Value);
            Assert.AreEqual("Sell", closedOrders[0].OrderSide.Value);
            Assert.AreEqual("Complete", closedOrders[0].Status.Value);

            Assert.AreEqual(0, closedOrders[1].Price.Value);
            Assert.AreEqual(3, closedOrders[1].Volume.Value);
            Assert.AreEqual(3, closedOrders[1].VolumeExecuted.Value);
            Assert.AreEqual(0, closedOrders[1].OpenQuantity.Value);
            Assert.AreEqual("Market", closedOrders[1].OrderType.Value);
            Assert.AreEqual("Buy", closedOrders[1].OrderSide.Value);
            Assert.AreEqual("Complete", closedOrders[1].Status.Value);

            Assert.AreEqual(253, closedOrders[2].Price.Value);
            Assert.AreEqual(2, closedOrders[2].Volume.Value);
            Assert.AreEqual(2, closedOrders[2].VolumeExecuted.Value);
            Assert.AreEqual(0, closedOrders[2].OpenQuantity.Value);
            Assert.AreEqual("Limit", closedOrders[2].OrderType.Value);
            Assert.AreEqual("Buy", closedOrders[2].OrderSide.Value);
            Assert.AreEqual("Complete", closedOrders[2].Status.Value);

            Assert.AreEqual(0, closedOrders[3].Price.Value);
            Assert.AreEqual(5, closedOrders[3].Volume.Value);
            Assert.AreEqual(5, closedOrders[3].VolumeExecuted.Value);
            Assert.AreEqual(0, closedOrders[3].OpenQuantity.Value);
            Assert.AreEqual("Market", closedOrders[3].OrderType.Value);
            Assert.AreEqual("Sell", closedOrders[3].OrderSide.Value);
            Assert.AreEqual("Complete", closedOrders[3].Status.Value);
            //------------------- Closed Orders -------------------------

            //------------------- Trades -------------------------

            dynamic trades= GetTrades(apiClient);          
            // This call return list of object, so we have to explicitly check values within elements
            Assert.AreEqual(252, trades[0][2].Value);// Price
            Assert.AreEqual(3, trades[0][3].Value);// Volume
            Assert.AreEqual("XBTUSD", trades[0][4].Value);

            Assert.AreEqual(252, trades[1][2].Value);
            Assert.AreEqual(2, trades[1][3].Value);
            Assert.AreEqual("XBTUSD", trades[0][4].Value);

            Assert.AreEqual(250, trades[2][2].Value);
            Assert.AreEqual(5, trades[2][3].Value);
            Assert.AreEqual("XBTUSD", trades[0][4].Value);
            //------------------- Trades -------------------------
        }

        [Test]
        [Category("Integration")]
        public void Scenario2Test_TestsScenario2AndItsOutcome_VerifiesThroughMarketDataOrderAndTradesResults()
        {
            ApiClient apiClient = GetClient();
            Scenario2OrderCreation(apiClient);

            // ------------------------------- Order Book ------------------------------
            dynamic orderBook = JsonConvert.DeserializeObject<dynamic>(apiClient.GetOrderBook("XBTUSD"));

            // Item1 = Bid Book, Item1[i].Item1 = Volume of 'i' Bid, Item1[i].Item2 = Price of 'i' Bid
            Assert.AreEqual(1, orderBook.m_Item1.Count);
            Assert.AreEqual(1, orderBook.m_Item1[0].m_Item1.Value);
            Assert.AreEqual(245, orderBook.m_Item1[0].m_Item2.Value);

            // Item2 = Bid Book, Item2[i].Item1 = Volume of Ask at index 'i', Item2[i].Item2 = Price of Bid at index 'i'
            Assert.AreEqual(0, orderBook.m_Item2.Count);
            // ------------------------------------------------------------------------

            // --------------------------------- Depth ---------------------------------
            dynamic depth = JsonConvert.DeserializeObject<dynamic>(apiClient.GetDepth("XBTUSD"));

            // Item1 = Bid Book, Item1.Item1 = Aggregated Volume, Item1.Item2 = Price, Item1.Item3 = Number of Orders
            Assert.AreEqual(1, depth.m_Item1[0].m_Item1.Value);
            Assert.AreEqual(245, depth.m_Item1[0].m_Item2.Value);
            Assert.AreEqual(1, depth.m_Item1[0].m_Item3.Value);
            Assert.IsNull(depth.m_Item1[1].Value);
            Assert.IsNull(depth.m_Item1[2].Value);
            Assert.IsNull(depth.m_Item1[3].Value);
            Assert.IsNull(depth.m_Item1[4].Value);

            // Item2 = Bid Book, Item2.Item1 = Aggregated Volume, Item2.Item2 = Price, Item2.Item3 = Number of Orders
            Assert.IsNull(depth.m_Item2[0].Value);
            Assert.IsNull(depth.m_Item2[1].Value);
            Assert.IsNull(depth.m_Item2[2].Value);
            Assert.IsNull(depth.m_Item2[3].Value);
            Assert.IsNull(depth.m_Item2[4].Value);

            // -----------------------------------------------------------------------

            //------------------------- Open Orders ----------------------------------
            dynamic openOrders = GetOpenOrders(apiClient);

            Assert.AreEqual(1, openOrders.Count);
            // First Open Order
            Assert.AreEqual(245, openOrders[0].Price.Value);
            Assert.AreEqual(8, openOrders[0].Volume.Value);
            Assert.AreEqual(7, openOrders[0].VolumeExecuted.Value);
            Assert.AreEqual(1, openOrders[0].OpenQuantity.Value);
            Assert.AreEqual("Limit", openOrders[0].OrderType.Value);
            Assert.AreEqual("Buy", openOrders[0].OrderSide.Value);
            Assert.AreEqual("PartiallyFilled", openOrders[0].Status.Value);
            //---------------------------------------------------------------------

            //-------------------------- Closed Orders ----------------------------
            dynamic closedOrders = GetClosedOrders(apiClient);
            Assert.AreEqual(0, closedOrders[0].Price.Value);
            Assert.AreEqual(10, closedOrders[0].Volume.Value);
            Assert.AreEqual(0, closedOrders[0].VolumeExecuted.Value);
            Assert.AreEqual(10, closedOrders[0].OpenQuantity.Value);
            Assert.AreEqual("Market", closedOrders[0].OrderType.Value);
            Assert.AreEqual("Buy", closedOrders[0].OrderSide.Value);
            Assert.AreEqual("Rejected", closedOrders[0].Status.Value);

            Assert.AreEqual(252, closedOrders[1].Price.Value);
            Assert.AreEqual(5, closedOrders[1].Volume.Value);
            Assert.AreEqual(3, closedOrders[1].VolumeExecuted.Value);
            Assert.AreEqual(2, closedOrders[1].OpenQuantity.Value);
            Assert.AreEqual("Limit", closedOrders[1].OrderType.Value);
            Assert.AreEqual("Sell", closedOrders[1].OrderSide.Value);
            Assert.AreEqual("Cancelled", closedOrders[1].Status.Value);

            Assert.AreEqual(250, closedOrders[2].Price.Value);
            Assert.AreEqual(7, closedOrders[2].Volume.Value);
            Assert.AreEqual(7, closedOrders[2].VolumeExecuted.Value);
            Assert.AreEqual(0, closedOrders[2].OpenQuantity.Value);
            Assert.AreEqual("Limit", closedOrders[2].OrderType.Value);
            Assert.AreEqual("Buy", closedOrders[2].OrderSide.Value);
            Assert.AreEqual("Complete", closedOrders[2].Status.Value);

            Assert.AreEqual(0, closedOrders[3].Price.Value);
            Assert.AreEqual(3, closedOrders[3].Volume.Value);
            Assert.AreEqual(3, closedOrders[3].VolumeExecuted.Value);
            Assert.AreEqual(0, closedOrders[3].OpenQuantity.Value);
            Assert.AreEqual("Market", closedOrders[3].OrderType.Value);
            Assert.AreEqual("Buy", closedOrders[3].OrderSide.Value);
            Assert.AreEqual("Complete", closedOrders[3].Status.Value);

            Assert.AreEqual(0, closedOrders[4].Price.Value);
            Assert.AreEqual(10, closedOrders[4].Volume.Value);
            Assert.AreEqual(0, closedOrders[4].VolumeExecuted.Value);
            Assert.AreEqual(10, closedOrders[4].OpenQuantity.Value);
            Assert.AreEqual("Market", closedOrders[4].OrderType.Value);
            Assert.AreEqual("Buy", closedOrders[4].OrderSide.Value);
            Assert.AreEqual("Rejected", closedOrders[4].Status.Value);

            Assert.AreEqual(0, closedOrders[5].Price.Value);
            Assert.AreEqual(5, closedOrders[5].Volume.Value);
            Assert.AreEqual(5, closedOrders[5].VolumeExecuted.Value);
            Assert.AreEqual(0, closedOrders[5].OpenQuantity.Value);
            Assert.AreEqual("Market", closedOrders[5].OrderType.Value);
            Assert.AreEqual("Sell", closedOrders[5].OrderSide.Value);
            Assert.AreEqual("Complete", closedOrders[5].Status.Value);

            Assert.AreEqual(0, closedOrders[6].Price.Value);
            Assert.AreEqual(4, closedOrders[6].Volume.Value);
            Assert.AreEqual(4, closedOrders[6].VolumeExecuted.Value);
            Assert.AreEqual(0, closedOrders[6].OpenQuantity.Value);
            Assert.AreEqual("Market", closedOrders[6].OrderType.Value);
            Assert.AreEqual("Sell", closedOrders[6].OrderSide.Value);
            Assert.AreEqual("Complete", closedOrders[6].Status.Value);

            Assert.AreEqual(0, closedOrders[7].Price.Value);
            Assert.AreEqual(5, closedOrders[7].Volume.Value);
            Assert.AreEqual(5, closedOrders[7].VolumeExecuted.Value);
            Assert.AreEqual(0, closedOrders[7].OpenQuantity.Value);
            Assert.AreEqual("Market", closedOrders[7].OrderType.Value);
            Assert.AreEqual("Sell", closedOrders[7].OrderSide.Value);
            Assert.AreEqual("Complete", closedOrders[7].Status.Value);
            //------------------------------------------------------------------------

            //------------------- Trades -------------------------

            dynamic trades = GetTrades(apiClient);
            // This call return list of object, so we have to explicitly check values within elements
            Assert.AreEqual(252, trades[0][2].Value);// Price
            Assert.AreEqual(3, trades[0][3].Value);// Volume
            Assert.AreEqual("XBTUSD", trades[0][4].Value);

            Assert.AreEqual(250, trades[1][2].Value);
            Assert.AreEqual(5, trades[1][3].Value);
            Assert.AreEqual("XBTUSD", trades[1][4].Value);

            Assert.AreEqual(250, trades[2][2].Value);
            Assert.AreEqual(2, trades[2][3].Value);
            Assert.AreEqual("XBTUSD", trades[2][4].Value);

            Assert.AreEqual(245, trades[3][2].Value);
            Assert.AreEqual(2, trades[3][3].Value);
            Assert.AreEqual("XBTUSD", trades[3][4].Value);

            Assert.AreEqual(245, trades[4][2].Value);
            Assert.AreEqual(5, trades[4][3].Value);
            Assert.AreEqual("XBTUSD", trades[4][4].Value);
            //-----------------------------------------------------------------------
        }

        [Test]
        [Category("Integration")]
        public void Scenario3Test_TestsScenario3AndItsOutcome_VerifiesThroughMarketDataOrderAndTradesResults()
        {
            ApiClient apiClient = GetClient();
            Scenario3OrderCreation(apiClient);

            // ------------------------------- Order Book ------------------------------
            dynamic orderBook = JsonConvert.DeserializeObject<dynamic>(apiClient.GetOrderBook("XBTUSD"));

            // Item1 = Bid Book, Item1[i].Item1 = Volume of 'i' Bid, Item1[i].Item2 = Price of 'i' Bid
            Assert.AreEqual(0, orderBook.m_Item1.Count);
            // Item2 = Bid Book, Item2[i].Item1 = Volume of Ask at index 'i', Item2[i].Item2 = Price of Bid at index 'i'
            Assert.AreEqual(0, orderBook.m_Item2.Count);
            // ------------------------------------------------------------------------

            // --------------------------------- Depth ---------------------------------
            dynamic depth = JsonConvert.DeserializeObject<dynamic>(apiClient.GetDepth("XBTUSD"));

            // Item1 = Bid Book, Item1.Item1 = Aggregated Volume, Item1.Item2 = Price, Item1.Item3 = Number of Orders
            Assert.IsNull(depth.m_Item1[0].Value);
            Assert.IsNull(depth.m_Item1[1].Value);
            Assert.IsNull(depth.m_Item1[2].Value);
            Assert.IsNull(depth.m_Item1[3].Value);
            Assert.IsNull(depth.m_Item1[4].Value);

            // Item2 = Bid Book, Item2.Item1 = Aggregated Volume, Item2.Item2 = Price, Item2.Item3 = Number of Orders
            Assert.IsNull(depth.m_Item2[0].Value);
            Assert.IsNull(depth.m_Item2[1].Value);
            Assert.IsNull(depth.m_Item2[2].Value);
            Assert.IsNull(depth.m_Item2[3].Value);
            Assert.IsNull(depth.m_Item2[4].Value);

            // -----------------------------------------------------------------------

            //------------------------- Open Orders ----------------------------------
            dynamic openOrders = GetOpenOrders(apiClient);
            Assert.IsNull(openOrders);
            //---------------------------------------------------------------------

            //-------------------------- Closed Orders ----------------------------
            dynamic closedOrders = GetClosedOrders(apiClient);
            Assert.AreEqual(252, closedOrders[0].Price.Value);
            Assert.AreEqual(5, closedOrders[0].Volume.Value);
            Assert.AreEqual(0, closedOrders[0].VolumeExecuted.Value);
            Assert.AreEqual(5, closedOrders[0].OpenQuantity.Value);
            Assert.AreEqual("Limit", closedOrders[0].OrderType.Value);
            Assert.AreEqual("Sell", closedOrders[0].OrderSide.Value);
            Assert.AreEqual("Cancelled", closedOrders[0].Status.Value);

            Assert.AreEqual(245, closedOrders[1].Price.Value);
            Assert.AreEqual(8, closedOrders[1].Volume.Value);
            Assert.AreEqual(0, closedOrders[1].VolumeExecuted.Value);
            Assert.AreEqual(8, closedOrders[1].OpenQuantity.Value);
            Assert.AreEqual("Limit", closedOrders[1].OrderType.Value);
            Assert.AreEqual("Buy", closedOrders[1].OrderSide.Value);
            Assert.AreEqual("Cancelled", closedOrders[1].Status.Value);

            Assert.AreEqual(250, closedOrders[2].Price.Value);
            Assert.AreEqual(7, closedOrders[2].Volume.Value);
            Assert.AreEqual(7, closedOrders[2].VolumeExecuted.Value);
            Assert.AreEqual(0, closedOrders[2].OpenQuantity.Value);
            Assert.AreEqual("Limit", closedOrders[2].OrderType.Value);
            Assert.AreEqual("Buy", closedOrders[2].OrderSide.Value);
            Assert.AreEqual("Complete", closedOrders[2].Status.Value);

            Assert.AreEqual(250, closedOrders[3].Price.Value);
            Assert.AreEqual(3, closedOrders[3].Volume.Value);
            Assert.AreEqual(2, closedOrders[3].VolumeExecuted.Value);
            Assert.AreEqual(1, closedOrders[3].OpenQuantity.Value);
            Assert.AreEqual("Limit", closedOrders[3].OrderType.Value);
            Assert.AreEqual("Buy", closedOrders[3].OrderSide.Value);
            Assert.AreEqual("Cancelled", closedOrders[3].Status.Value);

            Assert.AreEqual(253, closedOrders[4].Price.Value);
            Assert.AreEqual(5, closedOrders[4].Volume.Value);
            Assert.AreEqual(0, closedOrders[4].VolumeExecuted.Value);
            Assert.AreEqual(5, closedOrders[4].OpenQuantity.Value);
            Assert.AreEqual("Limit", closedOrders[4].OrderType.Value);
            Assert.AreEqual("Sell", closedOrders[4].OrderSide.Value);
            Assert.AreEqual("Cancelled", closedOrders[4].Status.Value);

            Assert.AreEqual(240, closedOrders[5].Price.Value);
            Assert.AreEqual(8, closedOrders[5].Volume.Value);
            Assert.AreEqual(0, closedOrders[5].VolumeExecuted.Value);
            Assert.AreEqual(8, closedOrders[5].OpenQuantity.Value);
            Assert.AreEqual("Limit", closedOrders[5].OrderType.Value);
            Assert.AreEqual("Buy", closedOrders[5].OrderSide.Value);
            Assert.AreEqual("Cancelled", closedOrders[5].Status.Value);

            Assert.AreEqual(245, closedOrders[6].Price.Value);
            Assert.AreEqual(7, closedOrders[6].Volume.Value);
            Assert.AreEqual(0, closedOrders[6].VolumeExecuted.Value);
            Assert.AreEqual(7, closedOrders[6].OpenQuantity.Value);
            Assert.AreEqual("Limit", closedOrders[6].OrderType.Value);
            Assert.AreEqual("Buy", closedOrders[6].OrderSide.Value);
            Assert.AreEqual("Cancelled", closedOrders[6].Status.Value);

            Assert.AreEqual(247, closedOrders[7].Price.Value);
            Assert.AreEqual(3, closedOrders[7].Volume.Value);
            Assert.AreEqual(0, closedOrders[7].VolumeExecuted.Value);
            Assert.AreEqual(3, closedOrders[7].OpenQuantity.Value);
            Assert.AreEqual("Limit", closedOrders[7].OrderType.Value);
            Assert.AreEqual("Buy", closedOrders[7].OrderSide.Value);
            Assert.AreEqual("Cancelled", closedOrders[7].Status.Value);

            Assert.AreEqual(0, closedOrders[8].Price.Value);
            Assert.AreEqual(9, closedOrders[8].Volume.Value);
            Assert.AreEqual(9, closedOrders[8].VolumeExecuted.Value);
            Assert.AreEqual(0, closedOrders[8].OpenQuantity.Value);
            Assert.AreEqual("Market", closedOrders[8].OrderType.Value);
            Assert.AreEqual("Sell", closedOrders[8].OrderSide.Value);
            Assert.AreEqual("Complete", closedOrders[8].Status.Value);
            //------------------------------------------------------------------------

            //------------------- Trades -------------------------

            dynamic trades = GetTrades(apiClient);
            // This call return list of object, so we have to explicitly check values within elements
            Assert.AreEqual(250, trades[0][2].Value);// Price
            Assert.AreEqual(7, trades[0][3].Value);// Volume
            Assert.AreEqual("XBTUSD", trades[0][4].Value);

            Assert.AreEqual(250, trades[1][2].Value);
            Assert.AreEqual(2, trades[1][3].Value);
            Assert.AreEqual("XBTUSD", trades[1][4].Value);
            //-----------------------------------------------------------------------
        }

        #region Client Methods

        /// <summary>
        /// Creates and returns the ApiClient
        /// </summary>
        /// <returns></returns>
        private ApiClient GetClient()
        {
            string baseUrl = "http://rockblanc.cloudapp.net/dev";
            baseUrl = "http://localhost:51780";
            ApiClient client = new ApiClient(baseUrl);
            System.Console.WriteLine("Requesting....");
            return client;
        }

        /// <summary>
        /// Testing scenario 1
        /// </summary>
        private void Scenario1OrderCreation(ApiClient client)
        {
            string currecyPair = "XBTUSD";
            //Create orders
            Console.WriteLine(client.CreateOrder(currecyPair, "limit", "buy", 10, 250));
            Thread.Sleep(1200);
            Console.WriteLine(client.CreateOrder(currecyPair, "limit", "sell", 5, 252));
            Thread.Sleep(1200);
            Console.WriteLine(client.CreateOrder(currecyPair, "market", "buy", 3));
            Thread.Sleep(1200);
            Console.WriteLine(client.CreateOrder(currecyPair, "limit", "buy", 2, 253));
            Thread.Sleep(1200);
            Console.WriteLine(client.CreateOrder(currecyPair, "market", "sell", 5));
            Thread.Sleep(1200);
            Console.WriteLine(client.CreateOrder(currecyPair, "limit", "buy", 2, 250));
            Thread.Sleep(5000);
        }

        /// <summary>
        /// Scenrio 2 order creation and sending
        /// </summary>
        /// <param name="client"></param>
        private static void Scenario2OrderCreation(ApiClient client)
        {
            string currecyPair = "XBTUSD";
            JObject joe = JObject.Parse(client.CreateOrder(currecyPair, "market", "buy", 10));
            Thread.Sleep(1200);
            joe.Property("OrderId").Value.ToString();
            string orderId2 = JObject.Parse(client.CreateOrder(currecyPair, "limit", "sell", 5, 252)).Property("OrderId").Value.ToString();
            Thread.Sleep(1200);
            JObject.Parse(client.CreateOrder(currecyPair, "limit", "buy", 8, 245)).Property("OrderId").Value.ToString();
            Thread.Sleep(1200);
            JObject.Parse(client.CreateOrder(currecyPair, "limit", "buy", 7, 250)).Property("OrderId").Value.ToString();
            Thread.Sleep(1200);
            JObject.Parse(client.CreateOrder(currecyPair, "market", "buy", 3)).Property("OrderId").Value.ToString();
            Thread.Sleep(2000);
            System.Console.WriteLine(client.CancelOrder(orderId2));
            Thread.Sleep(1200);
            JObject.Parse(client.CreateOrder(currecyPair, "market", "buy", 10)).Property("OrderId").Value.ToString();
            Thread.Sleep(1200);
            JObject.Parse(client.CreateOrder(currecyPair, "market", "sell", 5)).Property("OrderId").Value.ToString();
            Thread.Sleep(1200);
            System.Console.WriteLine(client.CancelOrder(orderId2));
            Thread.Sleep(1200);
            client.CreateOrder(currecyPair, "market", "sell", 4);
            Thread.Sleep(1200);
            client.CreateOrder(currecyPair, "market", "sell", 5);
            Thread.Sleep(3000);
        }

        /// <summary>
        /// Creates and sends order for Scenario 3
        /// </summary>
        /// <param name="client"></param>
        private static void Scenario3OrderCreation(ApiClient client)
        {
            string currecyPair = "XBTUSD";
            JObject joe = JObject.Parse(client.CreateOrder(currecyPair, "limit", "sell", 5, 252));
            Thread.Sleep(1200);
            string orderId1 = joe.Property("OrderId").Value.ToString();
            string orderId2 = JObject.Parse(client.CreateOrder(currecyPair, "limit", "buy", 8, 245)).Property("OrderId").Value.ToString();
            Thread.Sleep(1200);
            string orderId3 = JObject.Parse(client.CreateOrder(currecyPair, "limit", "buy", 7, 250)).Property("OrderId").Value.ToString();
            Thread.Sleep(1200);
            string orderId4 = JObject.Parse(client.CreateOrder(currecyPair, "limit", "buy", 3, 250)).Property("OrderId").Value.ToString();
            Thread.Sleep(1200);
            string orderId5 = JObject.Parse(client.CreateOrder(currecyPair, "limit", "sell", 5, 253)).Property("OrderId").Value.ToString();
            Thread.Sleep(1200);
            string orderId6 = JObject.Parse(client.CreateOrder(currecyPair, "limit", "buy", 8, 240)).Property("OrderId").Value.ToString();
            Thread.Sleep(1200);
            string orderId7 = JObject.Parse(client.CreateOrder(currecyPair, "limit", "buy", 7, 245)).Property("OrderId").Value.ToString();
            Thread.Sleep(1200);
            string orderId8 = JObject.Parse(client.CreateOrder(currecyPair, "limit", "buy", 3, 247)).Property("OrderId").Value.ToString();
            Thread.Sleep(2000);
            System.Console.WriteLine(client.CancelOrder(orderId6));
            Thread.Sleep(1200);
            System.Console.WriteLine(client.CancelOrder(orderId1));
            Thread.Sleep(1200);
            string orderId9 = client.CreateOrder(currecyPair, "market", "sell", 9);
            Thread.Sleep(1200);
            System.Console.WriteLine(client.CancelOrder(orderId8));
            Thread.Sleep(1200);
            System.Console.WriteLine(client.CancelOrder(orderId7));
            Thread.Sleep(1200);
            System.Console.WriteLine(client.CancelOrder(orderId6));
            Thread.Sleep(1200);
            System.Console.WriteLine(client.CancelOrder(orderId5));
            Thread.Sleep(1200);
            System.Console.WriteLine(client.CancelOrder(orderId4));
            Thread.Sleep(1200);
            System.Console.WriteLine(client.CancelOrder(orderId3));
            Thread.Sleep(1200);
            System.Console.WriteLine(client.CancelOrder(orderId2));
            Thread.Sleep(1200);
            System.Console.WriteLine(client.CancelOrder(orderId1));
            Thread.Sleep(3500);
        }

        /// <summary>
        /// Get Open Orders
        /// </summary>
        private Newtonsoft.Json.Linq.JArray GetOpenOrders(ApiClient apiClient)
        {
            Newtonsoft.Json.Linq.JArray deserializeObject = null;
            try
            {
                deserializeObject = JsonConvert.DeserializeObject<dynamic>(apiClient.QueryOpenOrdersParams(true, ""));
            }
            catch (JsonReaderException exception)
            {
                Console.WriteLine(exception);
            }
            return deserializeObject;
        }

        /// <summary>
        /// Get Closed Orders
        /// </summary>
        /// <param name="apiClient"></param>
        /// <returns></returns>
        private Newtonsoft.Json.Linq.JArray GetClosedOrders(ApiClient apiClient)
        {
            Newtonsoft.Json.Linq.JArray deserializeObject =
                JsonConvert.DeserializeObject<dynamic>(apiClient.QueryClosedOrdersParams(true, "", ""));
            return deserializeObject;
        }

        /// <summary>
        /// Get Trades
        /// </summary>
        /// <param name="apiClient"></param>
        /// <returns></returns>
        private Newtonsoft.Json.Linq.JArray GetTrades(ApiClient apiClient)
        {
            Newtonsoft.Json.Linq.JArray deserializeObject =
                JsonConvert.DeserializeObject<dynamic>(apiClient.QueryTrades("", false));
            return deserializeObject;
        }

        #endregion Client Methods
    }
}

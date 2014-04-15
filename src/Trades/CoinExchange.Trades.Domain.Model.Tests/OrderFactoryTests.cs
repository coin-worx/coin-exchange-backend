using System;
using CoinExchange.Trades.Domain.Model.Order;
using CoinExchange.Trades.Infrastructure.Services;
using NUnit.Framework;

namespace CoinExchange.Trades.Domain.Model.Tests
{
    [TestFixture]
    public class OrderFactoryTests
    {
        [SetUp]
        public void SetUp()
        {
            
        }

        [TearDown]
        public void TearDown()
        {
            
        }
        
        /// <summary>
        /// To verify buy side market order created
        /// </summary>
        [Test]
        [Category("Unit")]
        public void CreateBuySideMarketOrderTest()
        {
            Order.Order order = OrderFactory.CreateOrder("1234", "XBTUSD", "market", "buy", 5, 0,
                new StubbedOrderIdGenerator());
            Assert.NotNull(order.OrderId);
            Assert.AreEqual(order.CurrencyPair, "XBTUSD");
            Assert.AreEqual(order.OrderType, OrderType.Market);
            Assert.AreEqual(order.OrderSide, OrderSide.Buy);
            Assert.AreEqual(order.Volume.Value, 5);
        }

        /// <summary>
        /// To verify sell side market order created
        /// </summary>
        [Test]
        [Category("Unit")]
        public void CreateSellSideMarketOrderTest()
        {
            Order.Order order = OrderFactory.CreateOrder("1234", "XBTUSD", "market", "sell", 5, 0,
                new StubbedOrderIdGenerator());
            Assert.NotNull(order.OrderId);
            Assert.AreEqual(order.CurrencyPair, "XBTUSD");
            Assert.AreEqual(order.OrderType, OrderType.Market);
            Assert.AreEqual(order.OrderSide, OrderSide.Sell);
            Assert.AreEqual(order.Volume.Value, 5);
        }

        /// <summary>
        /// To verify buy side limit order created
        /// </summary>
        [Test]
        [Category("Unit")]
        public void CreateBuySideLimitOrderTest()
        {
            Order.Order order = OrderFactory.CreateOrder("1234", "XBTUSD", "limit", "buy", 5, 10,
                new StubbedOrderIdGenerator());
            Assert.NotNull(order.OrderId);
            Assert.AreEqual(order.CurrencyPair, "XBTUSD");
            Assert.AreEqual(order.OrderType, OrderType.Limit);
            Assert.AreEqual(order.OrderSide, OrderSide.Buy);
            Assert.AreEqual(order.Volume.Value, 5);
            Assert.AreEqual(order.Price.Value, 10);
        }

        /// <summary>
        /// To verify sell side limit order created
        /// </summary>
        [Test]
        [Category("Unit")]
        public void CreateSellSideLimitOrderTest()
        {
            Order.Order order = OrderFactory.CreateOrder("1234", "XBTUSD", "limit", "sell", 5, 10,
                new StubbedOrderIdGenerator());
            Assert.NotNull(order.OrderId);
            Assert.AreEqual(order.CurrencyPair, "XBTUSD");
            Assert.AreEqual(order.OrderType, OrderType.Limit);
            Assert.AreEqual(order.OrderSide, OrderSide.Sell);
            Assert.AreEqual(order.Volume.Value, 5);
            Assert.AreEqual(order.Price.Value, 10);
        }

        /// <summary>
        /// To verify order volume is greater than 0
        /// </summary>
        [Test]
        [Category("Unit")]
        public void InvalidOrderVolumeTest()
        {
            bool orderVolumeException = false;
            decimal volume = 0;
            try
            {
                Order.Order order = OrderFactory.CreateOrder("1234", "XBTUSD", "limit", "sell", volume, 10,
                    new StubbedOrderIdGenerator());
            }
            catch (InvalidOperationException exception)
            {
                orderVolumeException = true;
            }
            Assert.AreEqual(orderVolumeException,true);
        }

    }
}

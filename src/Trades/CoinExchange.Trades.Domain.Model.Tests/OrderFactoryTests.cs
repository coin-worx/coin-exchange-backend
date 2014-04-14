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
        /// Create order test case
        /// </summary>
        [Test]
        [Category("Unit")]
        public void CreateOrderTest()
        {
            Order.Order order = OrderFactory.CreateOrder("1234", "XBTUSD", "market", "buy", 5,0,new StubbedOrderIdGenerator());
            Assert.NotNull(order);
        }
        /// <summary>
        /// Test case to validate that all the fields are set properly
        /// </summary>
        [Test]
        [Category("Unit")]
        public void ValidateFieldsTestCase()
        {
            Order.Order order = OrderFactory.CreateOrder("1234", "XBTUSD", "market", "buy", 5, 0,
                new StubbedOrderIdGenerator());
            Assert.NotNull(order.OrderId);
            Assert.AreEqual(order.TraderId.Id.ToString(),"1234");
            Assert.AreEqual(order.CurrencyPair, "XBTUSD");
            Assert.AreEqual(order.OrderType,OrderType.Market);
            Assert.AreEqual(order.OrderSide,OrderSide.Buy);
            Assert.AreEqual(order.Volume,5);
        }
    }
}

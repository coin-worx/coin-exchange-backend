using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Trades.ReadModel.DTO;
using CoinExchange.Trades.ReadModel.Repositories;
using NHibernate.Criterion;
using NUnit.Framework;
using Spring.Context.Support;

namespace CoinExchange.Trades.ReadModel.Persistence.Tests
{
    [TestFixture]
    public class OrderPersistenceTests
    {
        private IPersistanceRepository _persistance;
        private IOrderRepository _orderRepository;
        [SetUp]
        public void Setup()
        {
            _persistance = ContextRegistry.GetContext()["PersistenceRepository"] as IPersistanceRepository;
            _orderRepository = ContextRegistry.GetContext()["OrderRepository"] as IOrderRepository;
        }

        [TearDown]
        public void TearDown()
        {
        }

        [Test]
        public void SaveOrderReadModel_IfSaveOrUpdateMethodIsCalled_ItShouldGetSavedInTheDatabase()
        {
            string id = DateTime.Now.ToString();
            OrderReadModel model=new OrderReadModel();
            model.OrderId = id;
            model.OrderSide = "Buy";
            model.OrderType = "market";
            model.CurrencyPair = "XBTUSD";
            model.Price = 0;
            model.Status = "Closed";
            model.TraderId = "234";
            model.VolumeExecuted = 123;
            _persistance.SaveOrUpdate(model);
            OrderReadModel getReadModel = _orderRepository.GetOrderById(id);
            Assert.NotNull(getReadModel);
            AssertAreEqual(getReadModel,model);
            
        }
        [Test]
        public void GetOpenOrders_IfTraderIdIsProvided_ItShouldRetireveAllOpenOrdersOfTrader()
        {
            string id = DateTime.Now.Millisecond.ToString();
            OrderReadModel model = new OrderReadModel();
            model.OrderId = id;
            model.OrderSide = "Buy";
            model.OrderType = "market";
            model.CurrencyPair = "XBTUSD";
            model.Price = 0;
            model.Status = "Open";
            model.TraderId = "999";
            model.VolumeExecuted = 123;
            _persistance.SaveOrUpdate(model);
            model.OrderId = DateTime.Now.Millisecond.ToString();
            _persistance.SaveOrUpdate(model);
            IList<OrderReadModel> getReadModel = _orderRepository.GetOpenOrders("999");
            bool check = true;
            Assert.NotNull(getReadModel);
            for (int i = 0; i < getReadModel.Count; i++)
            {
                if (!getReadModel[i].TraderId.Equals("999") || !getReadModel[i].Status.Equals("Open"))
                {
                    check = false;
                }
            }
            Assert.AreEqual(check,true);
        }

        [Test]
        public void GetClosedOrders_IfTraderIdIsProvided_ItShouldRetireveAllClosedOrdersOfTrader()
        {
            string id = DateTime.Now.Millisecond.ToString();
            OrderReadModel model = new OrderReadModel();
            model.OrderId = id;
            model.OrderSide = "Buy";
            model.OrderType = "market";
            model.CurrencyPair = "XBTUSD";
            model.Price = 0;
            model.Status = "Closed";
            model.TraderId = "999";
            model.VolumeExecuted = 123;
            _persistance.SaveOrUpdate(model);
            model.OrderId = DateTime.Now.Millisecond.ToString();
            _persistance.SaveOrUpdate(model);
            IList<OrderReadModel> getReadModel = _orderRepository.GetClosedOrders("999");
            bool check = true;
            Assert.NotNull(getReadModel);
            for (int i = 0; i < getReadModel.Count; i++)
            {
                if (!getReadModel[i].TraderId.Equals("999") || !getReadModel[i].Status.Equals("Closed"))
                {
                    check = false;
                }
            }
            Assert.AreEqual(check, true);
        }

        private void AssertAreEqual(OrderReadModel expected, OrderReadModel actual)
        {
            Assert.AreEqual(expected.OrderId,actual.OrderId);
            Assert.AreEqual(expected.OrderSide, actual.OrderSide);
            Assert.AreEqual(expected.OrderType, actual.OrderType);
            Assert.AreEqual(expected.Price, actual.Price);
            Assert.AreEqual(expected.Status, actual.Status);
            Assert.AreEqual(expected.TraderId, actual.TraderId);
            Assert.AreEqual(expected.VolumeExecuted, actual.VolumeExecuted);
            Assert.AreEqual(expected.CurrencyPair, actual.CurrencyPair);
        }
    }
}

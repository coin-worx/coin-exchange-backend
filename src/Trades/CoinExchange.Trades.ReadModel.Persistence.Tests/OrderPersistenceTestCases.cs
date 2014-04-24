using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Trades.ReadModel.DTO;
using CoinExchange.Trades.ReadModel.Repositories;
using NUnit.Framework;
using Spring.Context.Support;

namespace CoinExchange.Trades.ReadModel.Persistence.Tests
{
    [TestFixture]
    public class OrderPersistenceTestCases
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
            Assert.AreEqual(getReadModel.OrderId,id);
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
            model.TraderId = "TestTrader";
            model.VolumeExecuted = 123;
            _persistance.SaveOrUpdate(model);
            model.OrderId = DateTime.Now.Millisecond.ToString();
            _persistance.SaveOrUpdate(model);
            IList<OrderReadModel> getReadModel = _orderRepository.GetOpenOrders("TestTrader");
            bool check = true;
            Assert.NotNull(getReadModel);
            for (int i = 0; i < getReadModel.Count; i++)
            {
                if (!getReadModel[i].TraderId.Equals("TestTrader") || !getReadModel[i].Status.Equals("Open"))
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
            model.TraderId = "TestTrader";
            model.VolumeExecuted = 123;
            _persistance.SaveOrUpdate(model);
            model.OrderId = DateTime.Now.Millisecond.ToString();
            _persistance.SaveOrUpdate(model);
            IList<OrderReadModel> getReadModel = _orderRepository.GetClosedOrders("TestTrader");
            bool check = true;
            Assert.NotNull(getReadModel);
            for (int i = 0; i < getReadModel.Count; i++)
            {
                if (!getReadModel[i].TraderId.Equals("TestTrader") || !getReadModel[i].Status.Equals("Closed"))
                {
                    check = false;
                }
            }
            Assert.AreEqual(check, true);
        }
    }
}

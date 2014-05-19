using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Trades.Domain.Model.OrderAggregate;
using CoinExchange.Trades.Infrastructure.Services;
using CoinExchange.Trades.ReadModel.DTO;
using CoinExchange.Trades.ReadModel.Repositories;
using NHibernate;
using NHibernate.Criterion;
using NUnit.Framework;
using Spring.Context.Support;
using Order = CoinExchange.Trades.Domain.Model.OrderAggregate.Order;

namespace CoinExchange.Trades.ReadModel.Persistence.Tests
{
    [TestFixture]
    public class OrderPersistenceTests:AbstractConfiguration
    {
        private IPersistanceRepository _persistanceRepository;
        private IOrderRepository _orderRepository;

        /// <summary>
        /// Injected dependency to OrderRepository
        /// </summary>
        public IOrderRepository OrderRepository
        {
            set { _orderRepository = value; }
        }

        /// <summary>
        /// Injected dependency to Persistance Repository
        /// </summary>
        public IPersistanceRepository Persistance
        {
            set { _persistanceRepository = value; }
        }
        
        [Test]
        public void SaveOrderReadModel_IfSaveOrUpdateMethodIsCalled_ItShouldGetSavedInTheDatabase()
        {
            Order order = OrderFactory.CreateOrder("1234", "XBTUSD", "limit", "buy", 5, 10,
               new StubbedOrderIdGenerator());
            string id = DateTime.Now.ToString();
            OrderReadModel model = ReadModelAdapter.GetOrderReadModel(order);
            _persistanceRepository.SaveOrUpdate(model);
            OrderReadModel getReadModel = _orderRepository.GetOrderById(order.OrderId.Id.ToString());
            Assert.NotNull(getReadModel);
            AssertAreEqual(getReadModel, model);
        }
        [Test]
        public void GetOpenOrders_IfTraderIdIsProvided_ItShouldRetireveAllOpenOrdersOfTrader()
        {
            Order order = OrderFactory.CreateOrder("999", "XBTUSD", "limit", "buy", 5, 10,
               new StubbedOrderIdGenerator());
            Order order1 = OrderFactory.CreateOrder("100", "XBTUSD", "limit", "buy", 5, 10,
               new StubbedOrderIdGenerator());
            string id = DateTime.Now.ToString();
            OrderReadModel model = ReadModelAdapter.GetOrderReadModel(order);
            _persistanceRepository.SaveOrUpdate(model);
            OrderReadModel model1 = ReadModelAdapter.GetOrderReadModel(order1);
            _persistanceRepository.SaveOrUpdate(model1);
            IList<OrderReadModel> getReadModel = _orderRepository.GetOpenOrders("999");
            Assert.NotNull(getReadModel);
            Assert.AreEqual(getReadModel.Count,1);
            AssertAreEqual(getReadModel[0],model);
        }

        [Test]
        public void GetClosedOrders_IfTraderIdIsProvided_ItShouldRetireveAllClosedOrdersOfTrader()
        {
            Order order = OrderFactory.CreateOrder("999", "XBTUSD", "limit", "buy", 5, 10,
               new StubbedOrderIdGenerator());
            order.OrderState=OrderState.Complete;
            string id = DateTime.Now.ToString();
            OrderReadModel model = ReadModelAdapter.GetOrderReadModel(order);
            _persistanceRepository.SaveOrUpdate(model);
            IList<OrderReadModel> getReadModel = _orderRepository.GetClosedOrders("999");
            Assert.NotNull(getReadModel);
            Assert.AreEqual(getReadModel.Count, 1);
            AssertAreEqual(getReadModel[0], model);
        }

        private void AssertAreEqual(OrderReadModel expected, OrderReadModel actual)
        {
            Assert.AreEqual(expected.OrderId, actual.OrderId);
            Assert.AreEqual(expected.Side, actual.Side);
            Assert.AreEqual(expected.Type, actual.Type);
            Assert.AreEqual(expected.Price, actual.Price);
            Assert.AreEqual(expected.Status, actual.Status);
            Assert.AreEqual(expected.TraderId, actual.TraderId);
            Assert.AreEqual(expected.VolumeExecuted, actual.VolumeExecuted);
            Assert.AreEqual(expected.CurrencyPair, actual.CurrencyPair);
        }
    }
}

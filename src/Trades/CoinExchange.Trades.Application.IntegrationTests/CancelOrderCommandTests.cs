using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Common.Domain.Model;
using CoinExchange.Trades.Application.OrderServices;
using CoinExchange.Trades.Application.OrderServices.Commands;
using CoinExchange.Trades.Domain.Model.OrderAggregate;
using CoinExchange.Trades.Domain.Model.TradeAggregate;
using CoinExchange.Trades.Infrastructure.Services;
using CoinExchange.Trades.ReadModel.DTO;
using CoinExchange.Trades.ReadModel.Repositories;
using NUnit.Framework;

namespace CoinExchange.Trades.Application.IntegrationTests
{
    [TestFixture]
    public class CancelOrderCommandTests:AbstractDaoIntegrationTests
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
        [Category("Integration")]
        public void ValidateCancelOrderCommand_IfOrderIdAndTraderIdIsValidAndOrderStatusIsNotCompleteOrCancelled_CommandWillBeValidated()
        {
            Order order = OrderFactory.CreateOrder("1234", "XBTUSD", "limit", "buy", 5, 10,
               new StubbedOrderIdGenerator());
            OrderReadModel model = ReadModelAdapter.GetOrderReadModel(order);
            _persistanceRepository.SaveOrUpdate(model);
            CancelOrderCommand command=new CancelOrderCommand(order.OrderId,order.TraderId);
            ICancelOrderCommandValidation validation=new CancelOrderCommandValidation(_orderRepository);
            Assert.True(validation.ValidateCancelOrderCommand(command));
        }

        [Test]
        [Category("Integration")]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ValidateCancelOrderCommand_IfOrderIdAndTraderIdIsValidAndOrderStatusIsCancelled_InvalidOperationException()
        {
            Order order = OrderFactory.CreateOrder("1234", "XBTUSD", "limit", "buy", 5, 10,
               new StubbedOrderIdGenerator());
            order.OrderState=OrderState.Cancelled;
            OrderReadModel model = ReadModelAdapter.GetOrderReadModel(order);
            _persistanceRepository.SaveOrUpdate(model);
            CancelOrderCommand command = new CancelOrderCommand(order.OrderId, order.TraderId);
            ICancelOrderCommandValidation validation = new CancelOrderCommandValidation(_orderRepository);
            validation.ValidateCancelOrderCommand(command);
        }
        [Test]
        [Category("Integration")]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ValidateCancelOrderCommand_IfOrderIdAndTraderIdIsValidAndOrderStatusIsComplete_InvalidOperationException()
        {
            Order order = OrderFactory.CreateOrder("1234", "XBTUSD", "limit", "buy", 5, 10,
               new StubbedOrderIdGenerator());
            order.OrderState = OrderState.Complete;
            OrderReadModel model = ReadModelAdapter.GetOrderReadModel(order);
            _persistanceRepository.SaveOrUpdate(model);
            CancelOrderCommand command = new CancelOrderCommand(order.OrderId, order.TraderId);
            ICancelOrderCommandValidation validation = new CancelOrderCommandValidation(_orderRepository);
            validation.ValidateCancelOrderCommand(command);
        }

        [Test]
        [Category("Integration")]
        [ExpectedException(typeof(ArgumentException))]
        public void ValidateCancelOrderCommand_IfOrderIdIsInValid_InvalidOperationException()
        {
            Order order = OrderFactory.CreateOrder("1234", "XBTUSD", "limit", "buy", 5, 10,
               new StubbedOrderIdGenerator());
            order.OrderState = OrderState.Complete;
            OrderReadModel model = ReadModelAdapter.GetOrderReadModel(order);
            _persistanceRepository.SaveOrUpdate(model);
            CancelOrderCommand command = new CancelOrderCommand(new OrderId(1234), order.TraderId);
            ICancelOrderCommandValidation validation = new CancelOrderCommandValidation(_orderRepository);
            validation.ValidateCancelOrderCommand(command);
        }
        [Test]
        [Category("Integration")]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ValidateCancelOrderCommand_IfOrderIdIsNotOfTrader_InvalidOperationException()
        {
            Order order = OrderFactory.CreateOrder("1234", "XBTUSD", "limit", "buy", 5, 10,
               new StubbedOrderIdGenerator());
            order.OrderState = OrderState.Complete;
            OrderReadModel model = ReadModelAdapter.GetOrderReadModel(order);
            _persistanceRepository.SaveOrUpdate(model);
            CancelOrderCommand command = new CancelOrderCommand(order.OrderId, new TraderId(2233));
            ICancelOrderCommandValidation validation = new CancelOrderCommandValidation(_orderRepository);
            validation.ValidateCancelOrderCommand(command);
        }
    }
}

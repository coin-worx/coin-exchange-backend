using System;
using System.Threading;
using CoinExchange.Trades.Application.OrderServices;
using CoinExchange.Trades.Application.OrderServices.Commands;
using CoinExchange.Trades.Application.OrderServices.Representation;
using CoinExchange.Trades.Domain.Model.OrderAggregate;
using Disruptor;
using NUnit.Framework;
using Spring.Context.Support;

namespace CoinExchange.Trades.Application.Tests
{
    [TestFixture]
    public class CreateOrderCommandTests:IEventHandler<InputPayload>
    {
        private IOrderApplicationService _orderseService;
        private ManualResetEvent _manualResetEvent;
        private Order _receivedOrder;
        
        [SetUp]
        public void SetUp()
        {
            _orderseService = ContextRegistry.GetContext()["OrderApplicationService"] as IOrderApplicationService;
            InputDisruptorPublisher.InitializeDisruptor(new IEventHandler<InputPayload>[]{this});
            _manualResetEvent=new ManualResetEvent(false);
        }

        [TearDown]
        public void TearDown()
        {
            InputDisruptorPublisher.Shutdown();
        }

        [Test]
        public void CreateMarketOrder_SendCreateOrderCommand_ReturnNewOrderRepresentationAndPublishOrderToDisruptor()
        {
            NewOrderRepresentation representation=_orderseService.CreateOrder(new CreateOrderCommand(0, "market", "buy", "XBTUSD", 10, "1234"));
            _manualResetEvent.WaitOne(3000);
            Assert.AreEqual(representation.OrderId,_receivedOrder.OrderId.Id.ToString());
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void CreateOrder_SendCreateOrderCommandWithZeroVolume_ThrowException()
        {
            NewOrderRepresentation representation = _orderseService.CreateOrder(new CreateOrderCommand(0, "market", "buy", "XBTUSD", 0, "1234"));
            _manualResetEvent.WaitOne(3000);
        }

        [Test]
        [ExpectedException(typeof(NullReferenceException))]
        public void CreateOrder_SendCreateOrderCommandWithInvalidOrderType_ThrowException()
        {
            NewOrderRepresentation representation = _orderseService.CreateOrder(new CreateOrderCommand(0, "mar", "buy", "XBTUSD", 10, "1234"));
            _manualResetEvent.WaitOne(3000);
        }

        [Test]
        public void CreateLimitOrder_SendCreateOrderCommand_ReturnNewOrderRepresentationAndPublishOrderToDisruptor()
        {
            NewOrderRepresentation representation = _orderseService.CreateOrder(new CreateOrderCommand(10, "limit", "buy", "XBTUSD", 10, "1234"));
            _manualResetEvent.WaitOne(3000);
            Assert.AreEqual(representation.OrderId, _receivedOrder.OrderId.Id.ToString());
        }

        public void OnNext(InputPayload data, long sequence, bool endOfBatch)
        {
            if (data.IsOrder)
            {
                _receivedOrder = data.Order;
                _manualResetEvent.Set();
            }
        }
    }
}

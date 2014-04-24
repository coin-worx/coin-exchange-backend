using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CoinExchange.Trades.Application.OrderServices;
using CoinExchange.Trades.Application.OrderServices.Commands;
using CoinExchange.Trades.Application.OrderServices.Representation;
using CoinExchange.Trades.Domain.Model.OrderAggregate;
using CoinExchange.Trades.Domain.Model.Services;
using CoinExchange.Trades.Domain.Model.TradeAggregate;
using Disruptor;
using NUnit.Framework;
using Spring.Context.Support;

namespace CoinExchange.Trades.Application.Tests
{
    [TestFixture]
    public class CancelOrderCommandTests:IEventHandler<InputPayload>
    {
        private IOrderApplicationService _orderseService;
        private ManualResetEvent _manualResetEvent;
        private OrderCancellation _cancellation;

        [SetUp]
        public void SetUp()
        {
            _orderseService = ContextRegistry.GetContext()["OrderApplicationService"] as IOrderApplicationService;
            InputDisruptorPublisher.InitializeDisruptor(new IEventHandler<InputPayload>[] { this });
            _manualResetEvent = new ManualResetEvent(false);
        }

        [TearDown]
        public void TearDown()
        {
        }

        [Test]
        public void SendCancelOrderRequest_IfValidOrderAndTraderIdProvided_ReceiveResponseAndCommandShouldBePublishedOnDisruptor()
        {
            CancelOrderResponse response =
                _orderseService.CancelOrder(new CancelOrderCommand(new OrderId(12), new TraderId(12)));
            _manualResetEvent.WaitOne(3000);
            Assert.NotNull(response);
            Assert.NotNull(_cancellation);
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void SendCancelOrderRequest_IfTraderIdIsNull_ReceiveInvalidOperationException()
        {
            CancelOrderResponse response =
                _orderseService.CancelOrder(new CancelOrderCommand(new OrderId(12), null));
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void SendCancelOrderRequest_IfOrderIdIsNull_ReceiveInvalidOperationException()
        {
            CancelOrderResponse response =
                _orderseService.CancelOrder(new CancelOrderCommand(null, null));
        }
        public void OnNext(InputPayload data, long sequence, bool endOfBatch)
        {
            if (!data.IsOrder)
            {
                _cancellation = data.OrderCancellation;
                _manualResetEvent.Set();
            }
        }
    }
}

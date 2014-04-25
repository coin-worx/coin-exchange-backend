using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CoinExchange.Trades.Domain.Model.OrderAggregate;
using CoinExchange.Trades.Domain.Model.TradeAggregate;
using CoinExchange.Trades.Infrastructure.Services;
using Disruptor;
using NUnit.Framework;

namespace CoinExchange.Trades.Domain.Model.Tests
{
    [TestFixture]
    public class InputDisruptorTests : IEventHandler<InputPayload>
    {
        private InputDisruptorPublisher _publisher;
        private InputPayload _receivedPayload;
        private ManualResetEvent _manualResetEvent;
        private List<int> _receivedTraderId;
        private int _counter = 0;
        public void OnNext(InputPayload data, long sequence, bool endOfBatch)
        {
            _counter--;
            _receivedPayload=new InputPayload(){OrderCancellation = new OrderCancellation(),Order = new Order()};
            if (data.IsOrder)
            {
                data.Order.MemberWiseClone(_receivedPayload.Order);
                _receivedPayload.IsOrder = true;
                _receivedTraderId.Add(data.Order.TraderId.Id);
            }
            else
            {
                data.OrderCancellation.MemberWiseClone(_receivedPayload.OrderCancellation);
                _receivedPayload.IsOrder = false;
            }
            if (_counter == 0)
            {
                _manualResetEvent.Set();
            }
        }

        [SetUp]
        public void Setup()
        {
            InputDisruptorPublisher.InitializeDisruptor(new IEventHandler<InputPayload>[] { this });
            _manualResetEvent=new ManualResetEvent(false);
            _receivedTraderId=new List<int>();
        }

        [TearDown]
        public void TearDown()
        {
            
        }

        [Test]
        public void PublishOrder_IfOrderIsAddedInPayload_ReceiveOrderInPayloadInConsumer()
        {
            _counter = 1;//as sending only one message
            Order order = OrderFactory.CreateOrder("1234", "XBTUSD", "limit", "buy", 5, 10,
                new StubbedOrderIdGenerator());
            InputPayload payload = InputPayload.CreatePayload(order);
            InputDisruptorPublisher.Publish(payload);
            _manualResetEvent.WaitOne(2000);
            Assert.AreEqual(payload.Order, order);
        }

        [Test]
        public void PublishCancelOrder_IfCancelOrderIsAddedInPayload_ReceiveCancelOrderInPayloadInConsumer()
        {
            _counter = 1;//as sending only one message
            OrderCancellation cancelOrder=new OrderCancellation(new OrderId(123),new TraderId(123) );
            InputPayload payload = InputPayload.CreatePayload(cancelOrder);
            InputDisruptorPublisher.Publish(payload);
            _manualResetEvent.WaitOne(2000);
            Assert.AreEqual(payload.OrderCancellation, cancelOrder);
        }

        [Test]
        public void PublishOrders_ToCheckPayloadRefrenceDoesnotGetMixed_AllOrdersReceived()
        {
            _counter = 14;//as sending 15 messages
            List<int> list=new List<int>();
            for (int i = 1; i < 15; i++)
            {
                Order order = OrderFactory.CreateOrder(i.ToString(), "XBTUSD", "limit", "buy", 5, 10,
                  new StubbedOrderIdGenerator());
                InputPayload payload = InputPayload.CreatePayload(order);
                InputDisruptorPublisher.Publish(payload);  
                list.Add(i);
            }
            _manualResetEvent.WaitOne(10000); 
            Assert.AreEqual(CompareList(list), true);
        }

        private bool CompareList(List<int> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i] != _receivedTraderId[i])
                    return false;
            }
            return true;
        }
    }
}

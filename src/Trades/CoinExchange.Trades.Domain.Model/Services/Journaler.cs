using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using CoinExchange.Trades.Domain.Model.DomainEvents;
using CoinExchange.Trades.Domain.Model.OrderAggregate;
using CoinExchange.Trades.Domain.Model.OrderMatchingEngine;
using CoinExchange.Trades.Domain.Model.TradeAggregate;
using Disruptor;

namespace CoinExchange.Trades.Domain.Model.Services
{
    /// <summary>
    /// Journaler for saving events
    /// </summary>
    public class Journaler:IEventHandler<InputPayload>,IEventHandler<byte[]>
    {
        private IEventStore _eventStore;
        private InputPayload _receivedPayload;
        public Journaler(IEventStore eventStore)
        {
            _eventStore = eventStore;
        }

        public void OnNext(InputPayload data, long sequence, bool endOfBatch)
        {
            _receivedPayload = new InputPayload() { OrderCancellation = new OrderCancellation(), Order = new Order() };
            if (data.IsOrder)
            {
                data.Order.MemberWiseClone(_receivedPayload.Order);
                _receivedPayload.IsOrder = true;
                _eventStore.StoreEvent(_receivedPayload.Order);
            }
            else
            {
                data.OrderCancellation.MemberWiseClone(_receivedPayload.OrderCancellation);
                _receivedPayload.IsOrder = false;
                _eventStore.StoreEvent(_receivedPayload.OrderCancellation);
            }
        }

        public void OnNext(byte[] data, long sequence, bool endOfBatch)
        {
            object getObject = ByteArrayToObject(data);
            if (getObject is Order)
            {
                _eventStore.StoreEvent(getObject as Order);
            }
            if (getObject is Trade)
            {
                _eventStore.StoreEvent(getObject as Trade);
            }
            if (getObject is LimitOrderBook)
            {
                LimitOrderBookEvent.Raise(getObject as LimitOrderBook);
            }
            if (getObject is Depth)
            {
               DepthEvent.Raise(getObject as Depth);
            }
            if (getObject is BBO)
            {
                BBOEvent.Raise(getObject as BBO);
            }
        }

        private static Object ByteArrayToObject(byte[] arrBytes)
        {
            MemoryStream memStream = new MemoryStream();
            BinaryFormatter binForm = new BinaryFormatter();
            memStream.Write(arrBytes, 0, arrBytes.Length);
            memStream.Seek(0, SeekOrigin.Begin);
            Object obj = (Object)binForm.Deserialize(memStream);
            return obj;
        }
    }
}

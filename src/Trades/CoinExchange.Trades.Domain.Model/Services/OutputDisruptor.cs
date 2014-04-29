using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Common.Domain.Model;
using CoinExchange.Common.Utility;
using CoinExchange.Trades.Domain.Model.OrderAggregate;
using Disruptor;
using Disruptor.Dsl;

namespace CoinExchange.Trades.Domain.Model.Services
{
    public class OutputDisruptor
    {
        private static readonly int _ringSize = Constants.DISRUPTOR_RING_SIZE;
        private static Disruptor<byte[]> _disruptor;
        private static RingBuffer<byte[]> _ringBuffer;
        private static EventPublisher<byte[]> _publisher;

        public static void InitializeDisruptor(IEventHandler<byte[]>[] eventHandler)
        {
            if (_disruptor == null)
            {
                // Initialize Disruptor
                _disruptor = new Disruptor<byte[]>(() => new byte[Constants.OUTPUT_DISRUPTOR_BYTE_ARRAY_SIZE], _ringSize, TaskScheduler.Default);
            }
            else
            {
                // Shutdown disruptor if it was already running
                _disruptor.Shutdown();
            }

            // Add Consumer
            _disruptor.HandleEventsWith(eventHandler);
            // Start Disruptor
            _ringBuffer = _disruptor.Start();
            // Get Publisher
            _publisher = new EventPublisher<byte[]>(_ringBuffer);
        }

        /// <summary>
        /// ShutDown
        /// </summary>
        public static void ShutDown()
        {
            if (_disruptor != null)
            {
                // Shutdown disruptor if it was already running
                _disruptor.Shutdown();
                _disruptor = null;
            }
        }

        public static void Publish(object obj)
        {
            byte[] received = StreamConversion.ObjectToByteArray(obj);
            _publisher.PublishEvent((entry, sequenceNo) =>
            {
                //copy byte to diruptor ring byte
                Buffer.BlockCopy(received,0,entry,0,received.Length);
                return entry;
            });
        }
    }
}

using System.Threading.Tasks;
using CoinExchange.Common.Domain.Model;
using Disruptor;
using Disruptor.Dsl;

namespace CoinExchange.Trades.Domain.Model.OrderAggregate
{
    /// <summary>
    /// Input Disruptor Publisher
    /// </summary>
    public class InputDisruptorPublisher
    {
        private static readonly int _ringSize = Constants.DISRUPTOR_RING_SIZE;
        private static Disruptor<InputPayload> _disruptor;
        private static RingBuffer<InputPayload> _ringBuffer;
        private static EventPublisher<InputPayload> _publisher;

        public static void InitializeDisruptor(IEventHandler<InputPayload>[] eventHandler)
        {
            if (_disruptor == null)
            {
                // Initialize Disruptor
                _disruptor = new Disruptor<InputPayload>(() => new InputPayload(){OrderCancellation = new OrderCancellation(),Order = new Order()}, _ringSize, TaskScheduler.Default);
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
            _publisher = new EventPublisher<InputPayload>(_ringBuffer);
        }

        public static void Publish(InputPayload payload)
        {
            _publisher.PublishEvent((entry, sequenceNo) =>
            {
                //check if payload is order or cancel order
                if (payload.IsOrder)
                {
                    payload.Order.MemberWiseClone(entry.Order);
                    entry.IsOrder = true;
                }
                else
                {
                    payload.OrderCancellation.MemberWiseClone(entry.OrderCancellation);
                    entry.IsOrder = false;
                }
                return entry;
            });
        }

    }
}

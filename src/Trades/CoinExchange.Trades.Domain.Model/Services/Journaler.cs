using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Trades.Domain.Model.Order;
using Disruptor;

namespace CoinExchange.Trades.Domain.Model.Services
{
    /// <summary>
    /// Journaler for saving events
    /// </summary>
    public class Journaler:IEventHandler<InputPayload>
    {
        private IEventStore _eventStore;
        private InputPayload _receivedPayload;
        public Journaler(IEventStore eventStore)
        {
            _eventStore = eventStore;
        }

        public void OnNext(InputPayload data, long sequence, bool endOfBatch)
        {
            _receivedPayload = new InputPayload() { CancelOrder = new CancelOrder(), Order = new Order.Order() };
            if (data.IsOrder)
            {
                data.Order.MemberWiseClone(_receivedPayload.Order);
                _receivedPayload.IsOrder = true;
                _eventStore.StoreEvent(_receivedPayload.Order);
            }
            else
            {
                data.CancelOrder.MemberWiseClone(_receivedPayload.CancelOrder);
                _receivedPayload.IsOrder = false;
                _eventStore.StoreEvent(_receivedPayload.CancelOrder);
            }
        }
    }
}

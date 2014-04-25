using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinExchange.Trades.Domain.Model.DomainEvents;
using CoinExchange.Trades.Domain.Model.OrderAggregate;
using CoinExchange.Trades.Domain.Model.TradeAggregate;
using NEventStore;
using NEventStore.Dispatcher;

namespace CoinExchange.Trades.Infrastructure.Persistence.RavenDb
{
    public class ReceiveCommit : IDispatchCommits
    {
        /// <summary>
        /// Dispatch all the received events
        /// </summary>
        /// <param name="commit"></param>
        public void Dispatch(Commit commit)
        {
            foreach (var eventMessage in commit.Events)
            {
                if (eventMessage.Body is Order)
                {
                    OrderEvent.Raise(eventMessage.Body as Order);
                }
                if (eventMessage.Body is Trade)
                {
                    TradeEvent.Raise(eventMessage.Body as Trade);
                }
            }
        }

        public void Dispose()
        {
            
        }
    }
}

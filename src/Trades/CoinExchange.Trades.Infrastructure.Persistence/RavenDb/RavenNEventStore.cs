using System;
using System.Collections.Generic;
using System.Linq;
using CoinExchange.Common.Domain.Model;
using CoinExchange.Trades.Domain.Model.OrderAggregate;
using CoinExchange.Trades.Domain.Model.Services;
using NEventStore;

namespace CoinExchange.Trades.Infrastructure.Persistence.RavenDb
{
    public class RavenNEventStore:IEventStore
    {
        private static readonly Guid StreamId = Guid.NewGuid();
        private static IStoreEvents _store;
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger
     (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public RavenNEventStore()
        {
            _store = GetInitializedEventStore();
        }

        /// <summary>
        /// Store the Event
        /// </summary>
        /// <param name="blob"></param>
        /// <returns></returns>
        public bool StoreEvent(object blob)
        {
            try
            {
                OpenOrCreateStream(blob);    
            }
            catch (Exception exception)
            {
                log.Error(exception);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Initialize RavenDB NEventStore
        /// </summary>
        /// <returns></returns>
        private static IStoreEvents GetInitializedEventStore()
        {
            return Wireup.Init()
                .UsingRavenPersistence(Constants.RAVEN_DB_CONNECTIONSTRING_NAME)
                .DefaultDatabase(Constants.RAVEN_DB_DATABASE_NAME)
                .UsingAsynchronousDispatchScheduler()
                .Build();
        }

        /// <summary>
        /// Write event to the stream(RavenDB collection)
        /// </summary>
        /// <param name="event"></param>
        private static void OpenOrCreateStream(object @event)
        {
           using (var stream = _store.OpenStream(StreamId, 0, int.MaxValue))
            {
               stream.Add(new EventMessage { Body = @event });
               stream.CommitChanges(Guid.NewGuid());
            }
        }

        /// <summary>
        /// Get Event from collection
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public object GetEvent(string id)
        {
            List<EventMessage> collection;
            using (var stream = _store.OpenStream(StreamId, 0, int.MaxValue))
            {
                collection = stream.CommittedEvents.ToList();
                for (int i = 0; i < collection.Count; i++)
                {
                    if (collection[i].Body is Order)
                    {
                        Order order = collection[i].Body as Order;
                        if (order.OrderId.Id.ToString().Equals(id))
                        {
                            return order;
                        }
                    }
                }
                
            }
            return null;
        }
    }
}

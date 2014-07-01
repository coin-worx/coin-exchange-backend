using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using CoinExchange.Common.Domain.Model;
using CoinExchange.Common.Utility;
using CoinExchange.Trades.Domain.Model.OrderAggregate;
using CoinExchange.Trades.Domain.Model.OrderMatchingEngine;
using CoinExchange.Trades.Domain.Model.Services;
using CoinExchange.Trades.Domain.Model.TradeAggregate;
using NEventStore;
using NEventStore.Dispatcher;
//using NEventStore.Persistence.RavenPersistence;
//using Raven.Abstractions.Linq;
//using Raven.Imports.Newtonsoft.Json;
//using Raven.Imports.Newtonsoft.Json.Linq;
using Newtonsoft.Json.Linq;


namespace CoinExchange.Trades.Infrastructure.Persistence.RavenDb
{
    /// <summary>
    /// Raven NEvent Store
    /// </summary>
    public class RavenNEventStore : IEventStore
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger
        (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private  Guid _streamId;
        private  IStoreEvents _store;
        private  IEventStream _stream;
        private int _snaphost = 0;
        private Snapshot _lastSnaphot=null;
        private DateTime _loadFrom = Constants.LoadEventsFrom;
        private string _eventStore;
        
        /// <summary>
        /// Default Constructor
        /// </summary>
        public RavenNEventStore(string eventStore)
        {
            _eventStore = eventStore;
            _streamId=Guid.NewGuid();
            _store = GetInitializedEventStore(new ReceiveCommit(),eventStore);
            _stream = _store.OpenStream(_streamId, 0, int.MaxValue);
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
                Log.Error(exception);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Initialize RavenDB NEventStore
        /// </summary>
        /// <returns></returns>
        private IStoreEvents GetInitializedEventStore(IDispatchCommits commits,string eventStore)
        {
            if (eventStore.Equals(Constants.OUTPUT_EVENT_STORE))
            {
                return Wireup.Init()
                    .UsingRavenPersistence(Constants.RAVEN_DB_CONNECTIONSTRING_NAME)
                    .DefaultDatabase(eventStore)
                    .UsingAsynchronousDispatchScheduler(commits)
                    .Build();
            }
            return Wireup.Init()
                    .UsingRavenPersistence(Constants.RAVEN_DB_CONNECTIONSTRING_NAME)
                    .DefaultDatabase(eventStore)
                    .UsingAsynchronousDispatchScheduler(null)
                    .Build();
        }

        /// <summary>
        /// Write event to the stream(RavenDB collection)
        /// </summary>
        /// <param name="event"></param>
        private void OpenOrCreateStream(object @event)
        {
            //if (!ReplayService.ReplayMode)
            //{
                _stream.Add(new EventMessage {Body = @event});
                _stream.CommitChanges(Guid.NewGuid());
                if (@event is Order)
                {
                    Log.Debug("New order stored in Event Store. ID: " + (@event as Order).OrderId.Id +
                              " | Order state: "
                              + (@event as Order).OrderState);
                }
            //}
        }

        /// <summary>
        /// Get Event from collection
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public object GetEvent(string id)
        {
            List<EventMessage> collection;
            using (var stream = _store.OpenStream(_streamId, 0, int.MaxValue))
            {
                collection = stream.CommittedEvents.ToList();
                for (int i = 0; i < collection.Count; i++)
                {
                    if (collection[i].Body is Order)
                    {
                        Order order = collection[i].Body as Order;
                        if (order.OrderId.Id.ToString(CultureInfo.InvariantCulture).Equals(id))
                        {
                            return order;
                        }
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Gets all the orders placed residing inside the EventStore
        /// </summary>
        /// <returns></returns>
        public List<Order> GetAllOrders()
        {
            List<Order> orders = new List<Order>();
            List<EventMessage> collection;
            collection = _stream.CommittedEvents.ToList();
            for (int i = 0; i < collection.Count; i++)
            {
                if (collection[i].Body is Order)
                {
                    Order order = collection[i].Body as Order;
                    orders.Add(order);
                }
            }
            Log.Debug("Number of orders fetched from Event Store: " + orders.Count);
            if (!orders.Any())
            {
                orders = null;
            }
            return orders;
        }

        /// <summary>
        /// Removes all order from the NEventStore
        /// </summary>
        public void RemoveAllEvents()
        {
            _store.Advanced.Purge();
        }

        /// <summary>
        /// Gets the orders based on the specified CurrencyPair
        /// </summary>
        /// <returns></returns>
        public List<Order> GetOrdersByCurrencyPair(string currencyPair)
        {
            
            List<Order> orders = new List<Order>();
            //if (_lastSnaphot == null)
            //{
                List<EventMessage> collection;
                //collection = _stream.CommittedEvents.ToList();
                var events = _store.Advanced.GetFrom(_loadFrom).ToList();
                for (int i = 0; i < events.Count; i++)
                {
                    if (events[i].Events[0].Body is Order)
                    {
                        Order order = events[i].Events[0].Body as Order;
                        if (order.CurrencyPair == currencyPair)
                        {
                            orders.Add(order);
                        }
                    }
                }
            //}
            //else
            //{
            //    var events = _store.OpenStream(_lastSnaphot, int.MaxValue);
            //    List<EventMessage> collection;
            //    collection = events.UncommittedEvents.ToList();
            //    for (int i = 0; i < collection.Count;i++)
            //    {
            //        if (collection[i].Body is Order)
            //        {
            //            Order order = collection[i].Body as Order;
            //            if (order.CurrencyPair == currencyPair)
            //            {
            //                orders.Add(order);
            //            }
            //        }
            //    }
            //}
            Log.Debug("Number of orders fetched from Event Store: " + orders.Count);
            if (!orders.Any())
            {
                orders = null;
            }
            return orders;
        }

        /// <summary>
        /// Get trade Event from collection by order id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IList<Trade> GetTradeEventsFromOrderId(string id)
        {
            List<EventMessage> collection;
            List<Trade> trades=new List<Trade>();
            using (var stream = _store.OpenStream(_streamId, 0, int.MaxValue))
            {
                collection = stream.CommittedEvents.ToList();
                for (int i = 0; i < collection.Count; i++)
                {
                    if (collection[i].Body is Trade)
                    {
                        Trade trade = collection[i].Body as Trade;
                        if (trade.BuyOrder.OrderId.Id.ToString().Equals(id)||trade.SellOrder.OrderId.Id.ToString().Equals(id))
                        {
                            trades.Add(trade);
                        }
                    }
                }
            }
            return trades;
        }
        
        public IList<object> GetAllEvents()
        {
            List<object> readEvents=new List<object>();
            var events = _store.Advanced.GetFrom(DateTime.MinValue).ToList();
            for (int i = 0; i < events.Count; i++)
            {
                readEvents.Add(events[i].Events[0].Body);
            }
            return readEvents;
        }


        public void ShutDown()
        {
            _stream.Dispose();
            _store.Dispose();
        }

        /// <summary>
        /// Save Exchange snapshot
        /// </summary>
        /// <param name="exchangeEssentials"></param>
        public void SaveSnapshot(ExchangeEssentialsList exchangeEssentials)
        {
            if (_eventStore == Constants.OUTPUT_EVENT_STORE)
            {
                _store.Advanced.AddSnapshot(new Snapshot(_streamId, _snaphost++,
                    StreamConversion.ObjectToByteArray(exchangeEssentials)));
            }
        }

        /// <summary>
        /// Load last snaphost
        /// </summary>
        /// <returns></returns>
        public ExchangeEssentialsList LoadLastSnapshot()
        {
            Snapshot snapshot=null;
            var initialize=_store.Advanced.GetFrom(Constants.LastSnapshotSearch).ToList();
            for (int i = 0; i < initialize.Count; i++)
            {
                snapshot = _store.Advanced.GetSnapshot(initialize[i].StreamId, int.MaxValue);
            }
            _lastSnaphot = snapshot;
            if (snapshot != null)
            {
                JObject jObject = JObject.Parse(snapshot.Payload.ToString());
                byte[] array= jObject["$value"].ToObject<byte[]>();
                object list = StreamConversion.ByteArrayToObject(array);
                ExchangeEssentialsList exchangeEssentialsList = list as ExchangeEssentialsList;
                _loadFrom = exchangeEssentialsList.LastSnapshotDateTime;
                return exchangeEssentialsList;
            }
            return null;
        }
    }
}

/***************************************************************************** 
* Copyright 2016 Aurora Solutions 
* 
*    http://www.aurorasolutions.io 
* 
* Aurora Solutions is an innovative services and product company at 
* the forefront of the software industry, with processes and practices 
* involving Domain Driven Design(DDD), Agile methodologies to build 
* scalable, secure, reliable and high performance products.
* 
* Coin Exchange is a high performance exchange system specialized for
* Crypto currency trading. It has different general purpose uses such as
* independent deposit and withdrawal channels for Bitcoin and Litecoin,
* but can also act as a standalone exchange that can be used with
* different asset classes.
* Coin Exchange uses state of the art technologies such as ASP.NET REST API,
* AngularJS and NUnit. It also uses design patterns for complex event
* processing and handling of thousands of transactions per second, such as
* Domain Driven Designing, Disruptor Pattern and CQRS With Event Sourcing.
* 
* Licensed under the Apache License, Version 2.0 (the "License"); 
* you may not use this file except in compliance with the License. 
* You may obtain a copy of the License at 
* 
*    http://www.apache.org/licenses/LICENSE-2.0 
* 
* Unless required by applicable law or agreed to in writing, software 
* distributed under the License is distributed on an "AS IS" BASIS, 
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. 
* See the License for the specific language governing permissions and 
* limitations under the License. 
*****************************************************************************/


﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CoinExchange.Common.Domain.Model;
using CoinExchange.Common.Utility;
using CoinExchange.Trades.Domain.Model.OrderAggregate;
using CoinExchange.Trades.Domain.Model.OrderMatchingEngine;
using Disruptor;
using Disruptor.Dsl;
using log4net.Repository.Hierarchy;

namespace CoinExchange.Trades.Domain.Model.Services
{
    public class OutputDisruptor
    {
        // Get the Current Logger
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static readonly int _ringSize = Constants.DISRUPTOR_RING_SIZE;
        private static Disruptor<byte[]> _disruptor;
        private static RingBuffer<byte[]> _ringBuffer;
        private static EventPublisher<byte[]> _publisher;

        /// <summary>
        /// Dedicated task to consume Object for Output Disruptor
        /// </summary>
        private static Task _disruptorConsumerTask;

        /// <summary>
        /// Token source used with Disruptor Consumer Task
        /// </summary>
        private static CancellationTokenSource _disruptorConsumerCancellationToken;

        /// <summary>
        /// Holds all incoming Objects (Order, LimitOrderBook, BBO, Depth) until they can be processed
        /// </summary>
        private static ConcurrentQueue<Object> _disruptorConsumerQueue;

        /// <summary>
        /// Wraps the Disruptor Consumer concurrent queue
        /// </summary>
        private static BlockingCollection<Object> _disruptorConsumerCollection;

        public static void InitializeDisruptor(IEventHandler<byte[]>[] eventHandler)
        {
            if (_disruptor == null)
            {
                // Initialize Disruptor
                _disruptor = new Disruptor<byte[]>(() => new byte[Constants.OUTPUT_DISRUPTOR_BYTE_ARRAY_SIZE], _ringSize, TaskScheduler.Default);

                // Intialize Consumer Queue
                _disruptorConsumerQueue = new ConcurrentQueue<Object>();
                _disruptorConsumerCollection = new BlockingCollection<Object>(_disruptorConsumerQueue);

                // Initialize Consumer Token
                _disruptorConsumerCancellationToken = new CancellationTokenSource();

                // Consumes order notification messages from local collection
                _disruptorConsumerTask = Task.Factory.StartNew(ConsumeIncomingObjects, _disruptorConsumerCancellationToken.Token);
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

                // Shutdown Consumer thread
                _disruptorConsumerCancellationToken.Cancel();
                _disruptor = null;
            }
        }

        public static void Publish(object obj)
        {
            //Add to local Queue
            _disruptorConsumerCollection.Add(obj);
        }

        /// <summary>
        /// Consumes Incoming Objects from local Queue
        /// </summary>
        private static void ConsumeIncomingObjects()
        {
            try
            {
                while (true)
                {
                    // Break thread if cancellation is requested
                    if (_disruptorConsumerCancellationToken.IsCancellationRequested)
                    {
                        break;
                    }

                    // Consume notification
                    var incomingObject = _disruptorConsumerCollection.Take();

                    // Send Data to Output Disruptor for publishing
                    PublishOnDisruptor(incomingObject);
                }
            }
            catch (Exception exception)
            {
                Log.Error(exception);
            }
        }

        public static void PublishOnDisruptor(object obj)
        {
            byte[] received = StreamConversion.ObjectToByteArray(obj);
            
            _publisher.PublishEvent((entry, sequenceNo) =>
            {
                //copy byte to diruptor ring byte
                Buffer.BlockCopy(received, 0, entry, 0, received.Length);
                return entry;
            });
        }
    }
}

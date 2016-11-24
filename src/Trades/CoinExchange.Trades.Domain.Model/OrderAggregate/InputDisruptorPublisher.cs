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


﻿using System.Threading.Tasks;
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

        /// <summary>
        /// Shuts down the Disruptor
        /// </summary>
        public static void Shutdown()
        {
            if (_disruptor != null)
            {
                _disruptor.Shutdown();
                _disruptor = null;
            }
        }

        /// <summary>
        /// Publishes the event
        /// </summary>
        /// <param name="payload"></param>
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

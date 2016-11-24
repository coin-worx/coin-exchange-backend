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


﻿namespace CoinExchange.Trades.Domain.Model.OrderAggregate
{
    /// <summary>
    /// Input for disruptor for new order and cancel order requests
    /// </summary>
    public class InputPayload
    {
        private Order _order;
        private OrderCancellation _orderCancellation;
        private bool _isOrder;//if true means it is a new order, false=>cancel order

        public Order Order
        {
            get { return _order; }
            set
            {
                _order = value;
            }
        }

        public OrderCancellation OrderCancellation
        {
            get { return _orderCancellation; }
            set
            {
               _orderCancellation = value;
            }
        }
        
        public bool IsOrder
        {
            get { return _isOrder; }
            set { _isOrder = value; }
        }

        /// <summary>
        /// payload is either Order or CancelOrder
        /// </summary>
        /// <param name="payload"></param>
        /// <returns></returns>
        public static InputPayload CreatePayload(object payload)
        {
            InputPayload inputPayload = null;
            if (payload is Order)
            {
                inputPayload=new InputPayload();
                inputPayload.Order = payload as Order;
                inputPayload.IsOrder = true;
            }
            else if(payload is OrderCancellation)
            {
                inputPayload=new InputPayload();
                inputPayload.OrderCancellation = payload as OrderCancellation;
                inputPayload.IsOrder = false;
            }
            return inputPayload;
        }
    }
}

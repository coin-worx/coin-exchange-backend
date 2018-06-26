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


﻿
namespace CoinExchange.IdentityAccess.Domain.Model.UserAggregate
{
    /// <summary>
    /// Specifies which actions are susbcribed by the user for authorization using MFA
    /// </summary>
    public class MfaSubscriptions
    {
        private int _userId;
        private bool _loginMfaEnabled = false;
        private bool _depositMfaEnabled = false;
        private bool _withdrawalMfaEnabled = false;
        private bool _placeOrderMfaEnabled = false;
        private bool _cancelOrderMfaEnabled = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public MfaSubscriptions()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public MfaSubscriptions(int userId)
        {
            _userId = userId;
        }

        /// <summary>
        /// Subscribe to Login MFA
        /// </summary>
        /// <returns></returns>
        public bool SubscribeLoginMfa()
        {
            _loginMfaEnabled = true;
            return true;
        }

        /// <summary>
        /// Subscribe to Deposit MFA
        /// </summary>
        /// <returns></returns>
        public bool SubscribeDepositMfa()
        {
            _depositMfaEnabled = true;
            return true;
        }

        /// <summary>
        /// Subscribe to Withdrawal MFA
        /// </summary>
        /// <returns></returns>
        public bool SubscribeWithdrawalMfa()
        {
            _withdrawalMfaEnabled = true;
            return true;
        }

        /// <summary>
        /// Subscribe to Place Order MFA
        /// </summary>
        /// <returns></returns>
        public bool SubscribePlaceOrderMfa()
        {
            _placeOrderMfaEnabled = true;
            return true;
        }

        /// <summary>
        /// Subscribe to Cancel Order MFA
        /// </summary>
        /// <returns></returns>
        public bool SubscribeCancelOrderMfa()
        {
            _cancelOrderMfaEnabled = true;
            return true;
        }

        public int Id { get; private set; }

        /// <summary>
        /// User ID
        /// </summary>
        public int UserId
        {
            get { return _userId; }
            private set { _userId = value; }
        }

        /// <summary>
        /// Is MFA enabled for Login
        /// </summary>
        public bool LoginMfaEnabled
        {
            get { return _loginMfaEnabled; }
            private set { _loginMfaEnabled = value; }
        }

        /// <summary>
        /// Is MFA enabled for Deposit
        /// </summary>
        public bool DepositMfaEnabled
        {
            get { return _depositMfaEnabled; }
            private set { _depositMfaEnabled = value; }
        }

        /// <summary>
        /// Is MFA enabled for Withdrawal
        /// </summary>
        public bool WithdrawalMfaEnabled
        {
            get { return _withdrawalMfaEnabled; }
            private set { _withdrawalMfaEnabled = value; }
        }

        /// <summary>
        /// Is MFA enabled for Placing a new order
        /// </summary>
        public bool PlaceOrderMfaEnabled
        {
            get { return _placeOrderMfaEnabled; }
            set { _placeOrderMfaEnabled = value; }
        }

        /// <summary>
        /// Is MFA enabled for Cancelling an order
        /// </summary>
        public bool CancelOrderMfaEnabled
        {
            get { return _cancelOrderMfaEnabled; }
            set { _cancelOrderMfaEnabled = value; }
        }
    }
}
